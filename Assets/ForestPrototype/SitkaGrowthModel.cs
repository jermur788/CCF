using System;

// Growth-model versions (saved as ForestSaveData.growthModel, save v17).
//   0 Legacy: Sitka height dH/dt = 0.45 (1 - H/35); no adult mortality.
//     Kept for v1-16 saves and Reference Future v1.
//   1 SiteClassDensity: Sitka height follows the Irish Class III top-height
//     envelope (Scenario One site), and adult trees are thinned by stand
//     density pressure acting on suppressed trees.
public static class GrowthModel
{
    public const int Legacy = 0;
    public const int SiteClassDensity = 1;
    public const int Latest = SiteClassDensity;

    public static int Normalize(int model)
    {
        return model < Legacy ? Legacy : model > Latest ? Latest : model;
    }
}

// Growth model 1 parameters for Sitka spruce, with provenance.
public static class SitkaGrowthModel
{
    public const string SpeciesId = "sitka-spruce";

    // ----- Site height (Scenario One: Irish site Class III, average) -----
    // Lekwadi et al. 2012 Irish Sitka top-height model, form
    // H(t) = b0 (1 - exp(-b2 t))^b3. b2 and b3 are the published Class III
    // values [A]. The published rounded b0 does not reproduce the published
    // age-30 anchor, so b0 is derived here from that anchor (20.4 m) [A/C].
    public const double ClassIIIb2 = 0.042;
    public const double ClassIIIb3 = 1.563;
    public const double ClassIIIAge30TopHeightM = 20.4;
    public static readonly double ClassIIIb0 =
        ClassIIIAge30TopHeightM / Math.Pow(1.0 - Math.Exp(-ClassIIIb2 * 30.0), ClassIIIb3);

    // Class III top-height (site potential) envelope at a given age, metres.
    // Top height, not an individual-tree target.
    public static double SiteTopHeightM(double ageYears)
    {
        if (ageYears <= 0.0) return 0.0;
        return ClassIIIb0 * Math.Pow(1.0 - Math.Exp(-ClassIIIb2 * ageYears), ClassIIIb3);
    }

    // Individual height keeps its position relative to the envelope: a tree
    // at fraction r of site top height stays at r as the envelope rises, so
    // suppressed trees stay shorter than dominants and thinning does not move
    // site top height. No competition term in height.
    public static float NextHeight(float heightM, int ageYears, int years = 1)
    {
        double now = SiteTopHeightM(Math.Max(1, ageYears));
        double next = SiteTopHeightM(Math.Max(1, ageYears) + years);
        return now > 0.0 ? (float)(heightM * next / now) : heightM;
    }

    // ----- Stand density (maximum size-density relationship) -----
    // Comeau, White, Kerr & Hale (2010), Forestry 83: British Sitka maximum
    // size-density line log N = a - 2.063 log Dq, maximum SDI 1868 at
    // Dq = 25 cm [B: British empirical, transferred to Ireland].
    // Relative density = N/ha * (Dq / 25 cm)^2.063 / 1868 (dimensionless).
    public const double MaxSizeDensitySlope = 2.063;
    public const double MaximumSdi = 1868.0;
    public const double SdiReferenceDqCm = 25.0;

    public static double RelativeDensity(double stemsPerHectare, double quadraticMeanDbhCm)
    {
        if (stemsPerHectare <= 0.0 || quadraticMeanDbhCm <= 0.0) return 0.0;
        return stemsPerHectare * Math.Pow(quadraticMeanDbhCm / SdiReferenceDqCm, MaxSizeDensitySlope) / MaximumSdi;
    }

    // ----- Adult density mortality [C: calibration choice; see
    // Docs/Research/SitkaGrowthMortality/AdultMortalityCalibration.md] -----
    // Density pressure P = max(0, (RD - onset) / (1 - onset)).
    // Hazard zone: annual death probability of tree i =
    //   min(cap, strength * P^2 * S_i^4 / mean(S^4)),
    // with S the tree's current suppression 1 - 1/(1 + CI/Ci50) from the
    // existing Hegyi response. Normalising by the stand mean makes pressure set
    // how many die (expected rate strength * P^2) and suppression set which:
    // suppressed trees carry most risk, dominant released trees rarely die.
    // Boundary: the stand may not exceed the maximum size-density line
    // (RD 1, the British Sitka line); if it would, the most suppressed
    // survivors die until it is back on the line.
    public const double MortalityOnsetRelativeDensity = 0.6;   // [C] tested 0.5 / 0.6 / 0.7
    public const double MortalityStrength = 0.08;              // [C] tested 0.02 / 0.04 / 0.08
    public const double MaximumAnnualDeathProbability = 0.5;   // [C] numerical bound
    public const double MaximumRelativeDensity = 1.0;          // [B] maximum size-density line
    // [C] Vulnerability exponent: S^4 concentrates risk on suppressed trees
    // (S^2 let even-sized dominants die at about 2%/yr in fixture tests).
    // Normalisation keeps the expected stand rate independent of it.
    public const double SuppressionExponent = 4.0;

    public static double DensityPressure(double relativeDensity)
    {
        return Math.Max(0.0, (relativeDensity - MortalityOnsetRelativeDensity) / (1.0 - MortalityOnsetRelativeDensity));
    }

    public static double Vulnerability(double suppression)
    {
        return Math.Pow(Math.Max(0.0, Math.Min(1.0, suppression)), SuppressionExponent);
    }

    // meanVulnerability: stand mean of Vulnerability(S) over living trees.
    public static double AnnualDeathProbability(double densityPressure, double suppression, double meanVulnerability)
    {
        if (densityPressure <= 0.0 || meanVulnerability <= 0.0) return 0.0;
        return Math.Min(MaximumAnnualDeathProbability,
            MortalityStrength * densityPressure * densityPressure * Vulnerability(suppression) / meanVulnerability);
    }
}
