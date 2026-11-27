using System.Globalization;

namespace SitkaP1.Ui;

/// <summary>Active byte budgets for <c>/multi</c> JSON view and lane-filtered stand payloads.</summary>
internal readonly record struct MultiViewBudgetLimit(int MaxViewBytes, int MaxEcologyBytes);

/// <summary>
/// Scaled payload budget formula (spec §8.1 / T-2A2-07).
/// Two-area stands (<c>MA-1</c>, <c>MA-1-MIXED</c>) keep the exact unchanged 512 KiB / 150 KiB caps.
/// Stands with 3 or more areas scale linearly with genesis stems (<c>524,288 × ceil(stems / 300)</c>)
/// up to the 2.5 MiB ceiling (<c>2,621,440 B</c>) for both whole-stand <c>/view</c> and lane-filtered
/// <c>/api/multi/stand?lanes=...</c> reads so a per-lane read is never stricter than the whole-stand
/// read it replaces. The 0.64 ha presets remain under 512 KiB on 3-lane reads (~495 KB).
/// </summary>
internal static class MultiViewBudget
{
    internal const int DefaultViewBytes = 524_288;      // 512 KiB (frozen MA-1 / MA-1-MIXED cap and per-300-stem scaling unit)
    internal const int DefaultEcologyBytes = 153_600;   // 150 KiB
    internal const int CeilingViewBytes = 2_621_440;    // 2.5 MiB hard ceiling for >=3-area stands (whole-stand and lane-filtered reads)
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

    internal static MultiViewBudgetLimit ForLaneFiltered(int totalAreaCount, int genesisStems, int? testMaxViewBytesOverride = null)
    {
        // 512 KiB remains the frozen cap for two-area stands (MA-1 / MA-1-MIXED); for >=3-area stands
        // the spec §8 scaled budget governs both whole-stand and lane-filtered reads so a per-lane read
        // is never stricter than the whole-stand read it replaces.
        return ForStand(totalAreaCount, genesisStems, testMaxViewBytesOverride);
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
