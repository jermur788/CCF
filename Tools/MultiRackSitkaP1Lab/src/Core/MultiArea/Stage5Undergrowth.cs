namespace SitkaP1.Core;

/// <summary>
/// Bramble and bracken cover of one ground cell and the survival penalty it puts on short juveniles (spec §3 step 5a and
/// §3.1). Ported from Unity UnderstoreyCompetition @ 5e79e986 with the same float32 operation order; all numbers are
/// GAMEPLAY CALIBRATION. Bramble and bracken share one light target (Unity avoids inventing a type ranking).
/// </summary>
internal static class Stage5Undergrowth
{
    /// <summary>T = 0.8 · clamp01(site · soil) · (0.1 + 0.9 · clamp01(light)), with neutral site and soil (1.0).</summary>
    public static float Target(float light)
    {
        float site = Clamp01(Stage5Constants.SiteProductivity * Stage5Constants.SoilStability);
        float clampedLight = Clamp01(light);
        return Stage5Constants.UndergrowthTargetMaximum * site *
            (Stage5Constants.UndergrowthShadeTargetFraction + (1f - Stage5Constants.UndergrowthShadeTargetFraction) * clampedLight);
    }

    /// <summary>Year-0 cover of both bramble and bracken: 0.25 · T.</summary>
    public static float Initial(float light) => Target(light) * Stage5Constants.UndergrowthInitialFraction;

    /// <summary>One annual step toward the target: clamp01(lerp(cover, T, T &gt; cover ? 0.30 : 0.45)).</summary>
    public static float Step(float cover, float target) =>
        Clamp01(Lerp(cover, target, target > cover ? Stage5Constants.UndergrowthGain : Stage5Constants.UndergrowthLoss));

    /// <summary>
    /// p_veg = clamp01(0.35 · clamp01(max(bramble, bracken)) · clamp01((1.5 − h) / (1.5 − 0.3))), with h the cohort's
    /// pre-growth height: full penalty at or below 0.3 m, none at or above 1.5 m. No graminoids and no browse concealment.
    /// </summary>
    public static float VegetationLossProbability(float bramble, float bracken, float preGrowthHeightM)
    {
        float exposure = MathF.Max(bramble, bracken);
        return Clamp01(Stage5Constants.UndergrowthStrength * Clamp01(exposure) *
            Clamp01((Stage5Constants.UndergrowthEscapeHeightM - preGrowthHeightM) /
                (Stage5Constants.UndergrowthEscapeHeightM - Stage5Constants.UndergrowthFullHeightM)));
    }

    internal static float Clamp01(float value) => value < 0f ? 0f : value > 1f ? 1f : value;

    /// <summary>Unity Mathf.Lerp: a + (b − a) · clamp01(t).</summary>
    internal static float Lerp(float a, float b, float t) => a + (b - a) * Clamp01(t);
}
