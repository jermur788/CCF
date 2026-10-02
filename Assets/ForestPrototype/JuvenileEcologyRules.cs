using UnityEngine;

// Biological calculations shared by cohort and exact-individual storage.
// Origin never enters these rules. Population accounting, age/identity and
// placement remain the adapters' responsibility; no new calibration values.
public static class JuvenileEcologyRules
{
    public static float LightResponse(TreeSpeciesDefinition species, float light)
        => species.JuvenileLightResponse(light);

    // Preserve the legacy cohort's unrounded complement through the density
    // multiplication. Returning a float here prematurely rounds 1-mortality
    // and changes the protected lifecycle despite identical model parameters.
    public static double SurvivalResponse(TreeSpeciesDefinition species, float light)
    {
        if (species.UsesDistinctJuvenileLightResponses)
            return species.DistinctJuvenileSurvivalResponse(light);
        return LightResponse(species, light) < species.RegenPoorLightThreshold
            ? 1d - species.RegenMortalityUnderPoorLight : 1d;
    }

    public static void GrowHeight(ref float height, TreeSpeciesDefinition species, float light, float siteProductivity)
    {
        float response = LightResponse(species, light);
        height += species.RegenHeightGrowthMPerYear * response * siteProductivity;
    }

    public static bool Survives(TreeSpeciesDefinition species, float light, float deterministicRoll)
        // Individuals retain the existing float threshold against the existing
        // deterministic 24-bit roll; this is a storage/realisation boundary.
        => deterministicRoll < (float)SurvivalResponse(species, light);

    public static bool CanPromote(TreeSpeciesDefinition species, float height, float light)
        => height >= species.PromotionHeightM && light >= species.PromotionMinimumLight;

    // Existing [C] juvenile-to-tree proportions, shared by both promotion adapters.
    public static float PromotionDbhCm(float height) => Mathf.Clamp(height * 1.5f, 2f, 20f);
}
