using UnityEngine;

// Provenance tags used in tooltips:
// [A] empirical anchor from the Sitka report, [B] evidence-derived relationship,
// [C] simulation abstraction, [D] gameplay/model calibration (not a forestry constant).
[CreateAssetMenu(fileName = "TreeSpecies", menuName = "Forest Prototype/Tree Species")]
public sealed class TreeSpeciesDefinition : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string speciesId = "";
    [SerializeField] private string displayName = "";
    [SerializeField] private string latinName = "";

    [Header("Growth")]
    [Tooltip("[C] Potential annual DBH growth of an open-grown tree, cm/year. Simulation abstraction, calibratable.")]
    [SerializeField, Min(0f)] private float potentialDbhGrowthCmPerYear = 0.9f;
    [Tooltip("[C] Maximum DBH in cm. Simulation bound (Sitka can exceed 100 cm; the prototype stand starts near 65-95 cm).")]
    [SerializeField, Min(1f)] private float maxDbhCm = 120f;
    [Tooltip("[C] Potential annual height growth of an open-grown tree, m/year. Height is not multiplied by the competition factor.")]
    [SerializeField, Min(0f)] private float potentialHeightGrowthMPerYear = 0.45f;
    [Tooltip("[C] Maximum height in metres. Simulation bound.")]
    [SerializeField, Min(1f)] private float maxHeightM = 35f;

    [Header("Competition")]
    [Tooltip("[D] Hegyi competition index at which growth is halved. Model calibration, not a measured constant.")]
    [SerializeField, Min(0.01f)] private float ci50 = 3f;

    [Header("Crown")]
    [Tooltip("[B] Potential crown radius relationship: radius_m = intercept + slope * DBH_cm. Evidence-derived British relationship.")]
    [SerializeField] private float crownRadiusIntercept = 0.9415f;
    [SerializeField] private float crownRadiusPerCmDbh = 0.07635f;
    [Tooltip("[B] Use the evidence-derived Beech crown-area relationship instead of the linear Sitka relation.")]
    [SerializeField] private bool usePowerLawCrown;
    [Tooltip("[B] Crown-area log intercept: ln(CPA_m2) = intercept + slope * ln(DBH_cm).")]
    [SerializeField] private float crownAreaLogIntercept = 0.05f;
    [Tooltip("[B] Crown-area log slope: ln(CPA_m2) = intercept + slope * ln(DBH_cm).")]
    [SerializeField] private float crownAreaLogSlope = 1.01f;
    [Tooltip("[D] Fraction of the gap to the target crown radius closed per year. Model calibration.")]
    [SerializeField, Range(0.01f, 1f)] private float crownRelaxationPerYear = 0.15f;

    [Header("Timber")]
    [Tooltip("[D] Form-height ratio: form height = tree height x this factor. Calibration for the biological stem-volume interface, not a measured constant.")]
    [SerializeField, Range(0.2f, 0.8f)] private float formHeightRatio = 0.5f;

    [Header("Reproduction")]
    [Tooltip("[A] Age in years where seed production starts (research indicates onset around 20-25 years).")]
    [SerializeField, Min(0f)] private float maturityOnsetYears = 20f;
    [Tooltip("[C] Age where maturity reaches full seed production.")]
    [SerializeField, Min(0f)] private float maturityFullYears = 30f;
    [Tooltip("[C] Relative seed potential of a fully mature open-grown crown.")]
    [SerializeField, Min(0f)] private float seedPotentialPerMatureTree = 1f;
    [Tooltip("[B] Seed dispersal kernel scale in metres: K(d) = exp(-d / scale). Approximation for ~80% of seed within 60 m.")]
    [SerializeField, Min(1f)] private float seedDispersalScaleM = 20f;
    [Tooltip("[C] Dispersal cutoff in metres for performance; a computational abstraction.")]
    [SerializeField, Min(1f)] private float seedDispersalCutoffM = 150f;
    [Tooltip("[D] Probability a year is a good mast year (research suggests good crops every ~4-8 years, not a fixed timer).")]
    [SerializeField, Range(0f, 1f)] private float mastGoodProbability = 0.15f;
    [Tooltip("[D] Probability a year is a poor mast year.")]
    [SerializeField, Range(0f, 1f)] private float mastPoorProbability = 0.35f;
    [Tooltip("[C] Mast multipliers applied to per-tree seed potential.")]
    [SerializeField, Min(0f)] private float mastGoodMultiplier = 1.5f;
    [SerializeField, Min(0f)] private float mastNormalMultiplier = 1f;
    [SerializeField, Min(0f)] private float mastPoorMultiplier = 0.4f;

    [Header("Regeneration")]
    [Tooltip("[D] Seed rain at which establishment reaches ~63% of saturation. Model calibration; tuned so a single nearby mature crown is meaningful.")]
    [SerializeField, Min(0.001f)] private float seedSaturationS50 = 5f;
    [Tooltip("[B anchors, C interpolation] Relative light (0-1) response anchors. Below ~10% strongly suppressed, 10-20% slow, ~20-25% transition, ~50-60% strong juvenile growth, high exposure plateaus/declines.")]
    [SerializeField] private float[] lightResponseLight = { 0f, 0.1f, 0.2f, 0.25f, 0.55f, 1f };
    [SerializeField] private float[] lightResponseFactor = { 0f, 0.1f, 0.3f, 0.5f, 0.9f, 0.8f };
    [Tooltip("[C] Regeneration height growth in full light and full site productivity, m/year.")]
    [SerializeField, Min(0f)] private float regenHeightGrowthMPerYear = 0.35f;
    [Tooltip("[D] Height at which an aggregated cohort is promoted to an individual tree. Gameplay/simulation choice.")]
    [SerializeField, Min(0.5f)] private float promotionHeightM = 3.5f;
    [Tooltip("[C] Seedling height at establishment, metres.")]
    [SerializeField, Min(0.01f)] private float regenInitialHeightM = 0.15f;
    [Tooltip("[C] Relative density added per unit of establishment.")]
    [SerializeField, Min(0f)] private float regenDensityPerEstablishment = 0.5f;
    [Tooltip("[C] Maximum relative regeneration density per cell.")]
    [SerializeField, Min(0.01f)] private float regenDensityMax = 1.5f;
    [Tooltip("[D] Annual mortality fraction of regeneration under persistently poor light.")]
    [SerializeField, Range(0f, 1f)] private float regenMortalityUnderPoorLight = 0.2f;
    [Tooltip("[D] Light response below which regeneration is considered suppressed.")]
    [SerializeField, Range(0f, 1f)] private float regenPoorLightThreshold = 0.15f;
    [Tooltip("[A/B] Whether this species can produce and establish regeneration in the current simulation slice. Beech is disabled until its regeneration milestone.")]
    [SerializeField] private bool supportsRegeneration = true;

    [Header("Wind risk (diagnostic only)")]
    [Tooltip("[D] Stand wind susceptibility multiplier. The Irish empirical model is stand-level and is not used as annual individual mortality.")]
    [SerializeField, Min(0f)] private float standWindSusceptibility = 1f;
    [Tooltip("[D] Weight of recent opening on local wind risk.")]
    [SerializeField, Min(0f)] private float windOpeningWeight = 1f;
    [Tooltip("[D] Years for recent-thinning exposure to decay by half.")]
    [SerializeField, Min(0.1f)] private float windThinningHalfLifeYears = 3f;

    public string SpeciesId => speciesId;
    public string DisplayName => displayName;
    public string LatinName => latinName;
    public string FullName => string.IsNullOrEmpty(latinName) ? displayName : displayName + " (" + latinName + ")";

    public float PotentialDbhGrowthCmPerYear => potentialDbhGrowthCmPerYear;
    public float MaxDbhCm => maxDbhCm;
    public float PotentialHeightGrowthMPerYear => potentialHeightGrowthMPerYear;
    public float MaxHeightM => maxHeightM;
    public float Ci50 => ci50;
    public float FormHeightRatio => formHeightRatio;
    public float CrownRadiusIntercept => crownRadiusIntercept;
    public float CrownRadiusPerCmDbh => crownRadiusPerCmDbh;
    public bool UsesPowerLawCrown => usePowerLawCrown;
    public float CrownRelaxationPerYear => crownRelaxationPerYear;
    public float MaturityOnsetYears => maturityOnsetYears;
    public float MaturityFullYears => maturityFullYears;
    public float SeedPotentialPerMatureTree => seedPotentialPerMatureTree;
    public float SeedDispersalScaleM => seedDispersalScaleM;
    public float SeedDispersalCutoffM => seedDispersalCutoffM;
    public float MastGoodProbability => mastGoodProbability;
    public float MastPoorProbability => mastPoorProbability;
    public float MastGoodMultiplier => mastGoodMultiplier;
    public float MastNormalMultiplier => mastNormalMultiplier;
    public float MastPoorMultiplier => mastPoorMultiplier;
    public float SeedSaturationS50 => seedSaturationS50;
    public float RegenHeightGrowthMPerYear => regenHeightGrowthMPerYear;
    public float PromotionHeightM => promotionHeightM;
    public float RegenInitialHeightM => regenInitialHeightM;
    public float RegenDensityPerEstablishment => regenDensityPerEstablishment;
    public float RegenDensityMax => regenDensityMax;
    public float RegenMortalityUnderPoorLight => regenMortalityUnderPoorLight;
    public float RegenPoorLightThreshold => regenPoorLightThreshold;
    public bool SupportsRegeneration => supportsRegeneration;
    public float StandWindSusceptibility => standWindSusceptibility;
    public float WindOpeningWeight => windOpeningWeight;
    public float WindThinningHalfLifeYears => windThinningHalfLifeYears;

    public float PotentialCrownRadiusM(float dbhCm)
    {
        if (usePowerLawCrown)
        {
            float dbh = Mathf.Max(0.1f, dbhCm);
            float crownArea = Mathf.Exp(crownAreaLogIntercept + crownAreaLogSlope * Mathf.Log(dbh));
            return Mathf.Max(0.1f, Mathf.Sqrt(crownArea / Mathf.PI));
        }
        return Mathf.Max(0.1f, crownRadiusIntercept + crownRadiusPerCmDbh * dbhCm);
    }

    public float Maturity(int ageYears)
    {
        if (ageYears <= maturityOnsetYears)
            return 0f;
        if (ageYears >= maturityFullYears)
            return 1f;
        return Mathf.InverseLerp(maturityOnsetYears, maturityFullYears, ageYears);
    }

    // Piecewise-linear interpolation between light-response anchors; [C] simulation design.
    public float JuvenileLightResponse(float relativeLight)
    {
        if (lightResponseLight == null || lightResponseFactor == null || lightResponseLight.Length < 2)
            return Mathf.Clamp01(relativeLight);
        int count = Mathf.Min(lightResponseLight.Length, lightResponseFactor.Length);
        float light = Mathf.Clamp01(relativeLight);
        if (light <= lightResponseLight[0])
            return lightResponseFactor[0];
        for (int i = 1; i < count; i++)
        {
            if (light <= lightResponseLight[i])
            {
                float t = Mathf.InverseLerp(lightResponseLight[i - 1], lightResponseLight[i], light);
                return Mathf.Lerp(lightResponseFactor[i - 1], lightResponseFactor[i], t);
            }
        }
        return lightResponseFactor[count - 1];
    }
}
