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

    // ----- Browsing & Protection v1 (Docs/BrowsingProtectionV1.md) -----
    // One browsing response for every juvenile representation. Cohorts apply
    // it to the expected browsed fraction; exact individuals realise one
    // deterministic annual event. A zero fraction leaves the light-only
    // arithmetic above untouched, so browse pressure 0 is bit-identical.

    // [C] Upper bound on the annual browse probability: even under extreme
    // pressure a few leaders escape in any one year.
    public const float BrowseProbabilityCap = 0.95f;
    // [C] Share of the year's potential height increment lost when the leader
    // is browsed. Major sensitivity parameter; light still sets the potential.
    public const float BrowseHeightIncrementLoss = 0.9f;
    // [C] Extra annual mortality for a browsed juvenile. Deliberately small:
    // browsing should mainly hold juveniles below escape height, not kill them.
    public const float BrowseExtraMortality = 0.04f;

    // [S] Full vulnerability up to the species' full-vulnerability height, a
    // linear taper to zero at its escape height (deer reach / leader decline).
    public static float BrowseHeightVulnerability(TreeSpeciesDefinition species, float height)
    {
        float full = species.BrowseFullVulnerabilityHeightM;
        float escape = species.BrowseEscapeHeightM;
        if (height <= full)
            return 1f;
        if (height >= escape)
            return 0f;
        return 1f - (height - full) / (escape - full);
    }

    // [S] effective pressure = background x palatability x height vulnerability
    // x vegetation exposure x protection access; p = min(effective, cap).
    public static BrowseAssessment AssessBrowse(TreeSpeciesDefinition species, float height, float backgroundPressure,
        float protectionAccess, BrowseProtectionState protection, float vegetationExposure = 1f)
    {
        var assessment = new BrowseAssessment
        {
            BackgroundPressure = Mathf.Clamp01(backgroundPressure),
            Palatability = species != null ? species.BrowsePalatability : 0f,
            HeightVulnerability = species != null ? BrowseHeightVulnerability(species, height) : 0f,
            ProtectionAccess = Mathf.Clamp01(protectionAccess),
            VegetationExposure = Mathf.Clamp01(vegetationExposure),
            Protection = protection
        };
        float effective = assessment.BackgroundPressure * assessment.Palatability * assessment.HeightVulnerability
            * assessment.VegetationExposure * assessment.ProtectionAccess;
        assessment.Probability = Mathf.Min(BrowseProbabilityCap, Mathf.Max(0f, effective));
        assessment.Reason = assessment.BackgroundPressure <= 0f ? BrowseExposureReason.NoBrowsePressure
            : assessment.Palatability <= 0f ? BrowseExposureReason.NotPalatable
            : assessment.HeightVulnerability <= 0f ? BrowseExposureReason.AboveBrowseReach
            : assessment.ProtectionAccess <= 0f ? BrowseExposureReason.Protected
            : BrowseExposureReason.Exposed;
        return assessment;
    }

    // Height growth for a juvenile population of which browsedFraction lost
    // their leader this year (a cohort's expected fraction, or 0/1 for an
    // individual). Browsing only removes increment; it never adds growth.
    public static void GrowHeight(ref float height, TreeSpeciesDefinition species, float light, float siteProductivity,
        float browsedFraction)
    {
        if (browsedFraction <= 0f)
        {
            GrowHeight(ref height, species, light, siteProductivity);
            return;
        }
        float response = LightResponse(species, light);
        float increment = species.RegenHeightGrowthMPerYear * response * siteProductivity;
        height += increment * (1f - Mathf.Clamp01(browsedFraction) * BrowseHeightIncrementLoss);
    }

    // Light survival combined with browse mortality of the browsed fraction.
    public static double SurvivalResponse(TreeSpeciesDefinition species, float light, float browsedFraction)
    {
        double survival = SurvivalResponse(species, light);
        if (browsedFraction <= 0f)
            return survival;
        return survival * (1d - Mathf.Clamp01(browsedFraction) * (double)BrowseExtraMortality);
    }

    public static bool Survives(TreeSpeciesDefinition species, float light, float deterministicRoll, bool browsed)
        => browsed
            ? deterministicRoll < (float)SurvivalResponse(species, light, 1f)
            : Survives(species, light, deterministicRoll);

    // Individual realisation of the shared probability. The roll id is a
    // browse-specific domain of the existing SimulationRandom.Roll, so it is
    // independent of the survival roll and needs no new RNG model.
    public static bool RealiseBrowse(float probability, int rngModel, string individualId, int year, int simulationSeed)
        => probability > 0f && SimulationRandom.Roll(rngModel, individualId + "/browse", year, simulationSeed) < probability;
}
