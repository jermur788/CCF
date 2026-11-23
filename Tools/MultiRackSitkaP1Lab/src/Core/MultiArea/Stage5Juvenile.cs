namespace SitkaP1.Core;

/// <summary>
/// Juvenile Sitka light response, height growth, survival and browsing (spec §3.2), ported from Unity JuvenileEcologyRules
/// and TreeSpeciesDefinition @ 5e79e986 with the same float32/double operation order. With a browsed fraction of 0 every
/// function takes Unity's exact light-only path, so browse pressure 0 is bit-identical to no browsing.
/// </summary>
internal static class Stage5Juvenile
{
    /// <summary>
    /// Piecewise-linear light response through (0,0) (0.10,0.10) (0.20,0.30) (0.25,0.50) (0.55,0.90) (1.00,0.80)
    /// (Unity InterpolateResponse: clamp the light, InverseLerp within the segment, then Lerp).
    /// </summary>
    public static float LightResponse(float relativeLight)
    {
        var lights = Stage5Constants.LightResponseLight;
        var factors = Stage5Constants.LightResponseFactor;
        float light = Stage5Undergrowth.Clamp01(relativeLight);
        if (light <= lights[0]) return factors[0];
        for (int i = 1; i < lights.Length; i++)
        {
            if (light <= lights[i])
            {
                float t = InverseLerp(lights[i - 1], lights[i], light);
                return Stage5Undergrowth.Lerp(factors[i - 1], factors[i], t);
            }
        }
        return factors[^1];
    }

    /// <summary>V(h): 1 up to 0.5 m, a linear taper to 0 at 0.8 m, then 0 (Unity BrowseHeightVulnerability).</summary>
    public static float BrowseHeightVulnerability(float heightM)
    {
        const float full = Stage5Constants.BrowseFullVulnerabilityHeightM;
        const float escape = Stage5Constants.BrowseEscapeHeightM;
        if (heightM <= full) return 1f;
        if (heightM >= escape) return 0f;
        return 1f - (heightM - full) / (escape - full);
    }

    /// <summary>
    /// p = min(0.95, pressure · palatability 0.15 · V(h)) at the pre-growth height (owner Q6). Unity's vegetation exposure
    /// and protection access are both 1 here (no concealment, no fences), so they are left out of the product.
    /// </summary>
    public static float BrowseProbability(float backgroundPressure, float preGrowthHeightM)
    {
        float effective = Stage5Undergrowth.Clamp01(backgroundPressure) * Stage5Constants.BrowsePalatability *
            BrowseHeightVulnerability(preGrowthHeightM);
        return MathF.Min(Stage5Constants.BrowseProbabilityCap, MathF.Max(0f, effective));
    }

    /// <summary>
    /// The new height after one year: h + 0.35 · LR(light) · site, the increment reduced by 0.90 · p when browsed.
    /// A zero browsed fraction takes the light-only expression exactly.
    /// </summary>
    public static float GrowHeight(float heightM, float light, float browsedFraction)
    {
        float response = LightResponse(light);
        if (browsedFraction <= 0f)
            return heightM + Stage5Constants.RegenHeightGrowthMPerYear * response * Stage5Constants.SiteProductivity;
        float increment = Stage5Constants.RegenHeightGrowthMPerYear * response * Stage5Constants.SiteProductivity;
        return heightM + increment * (1f - Stage5Undergrowth.Clamp01(browsedFraction) * Stage5Constants.BrowseHeightIncrementLoss);
    }

    /// <summary>
    /// Light survival in double: 1 − 0.2 when LR(light) &lt; 0.15, else 1 (Unity keeps the unrounded double complement of
    /// the float 0.2f, so the value is 1 − (double)0.2f, not exactly 0.8).
    /// </summary>
    public static double LightSurvival(float light) =>
        LightResponse(light) < Stage5Constants.RegenPoorLightThreshold ? 1d - Stage5Constants.RegenMortalityUnderPoorLight : 1d;

    /// <summary>Light survival times (1 − 0.04 · p) for a browsed fraction p &gt; 0; exactly the light value when p is 0.</summary>
    public static double LightBrowseSurvival(float light, float browsedFraction)
    {
        double survival = LightSurvival(light);
        if (browsedFraction <= 0f) return survival;
        return survival * (1d - Stage5Undergrowth.Clamp01(browsedFraction) * (double)Stage5Constants.BrowseExtraMortality);
    }

    /// <summary>
    /// The whole annual survival of a band (spec §3.2, double precision): light · (1 − 0.04·p) · (1 − p_veg). With
    /// p_veg = 0 this is exactly <see cref="LightBrowseSurvival"/>.
    /// </summary>
    public static double Survival(float light, float browsedFraction, float vegetationLossProbability)
    {
        double survival = LightBrowseSurvival(light, browsedFraction);
        if (vegetationLossProbability <= 0f) return survival;
        return survival * (1d - vegetationLossProbability);
    }

    /// <summary>Unity Mathf.InverseLerp: clamp01((v − a) / (b − a)), 0 when a == b.</summary>
    internal static float InverseLerp(float a, float b, float value) =>
        a != b ? Stage5Undergrowth.Clamp01((value - a) / (b - a)) : 0f;
}
