namespace SitkaP1.Core;

/// <summary>
/// Broadleaf adult growth and C2 crowns (spec §3.1–3.2). Sitka never comes here: it keeps the unchanged Stage 4 step.
/// Ported from Unity ForestEcologyController.GrowAdults (legacy height law for non-Sitka species) with the same float32
/// operation order; the DBH/height clamps are the engine's existing adult domain [1, 200] cm and [0.1, 60] m.
/// </summary>
internal static class Phase2Adult
{
    /// <summary>
    /// One year of a living Oak or Beech adult:
    ///   dDBH = Pd · site · clamp01(1 − DBH/Dmax) · (1 / (1 + CI/Ci50_species))
    ///   dH   = Ph · site · clamp01(1 − H/Hmax)
    /// with CI the connected-woodland 8 m Hegyi index (species-neutral), then the C2 crown step toward the pruned target.
    /// </summary>
    public static AnnualTree Grow(AnnualTree tree, Phase2Species species, float woodlandCi, float site, int pruningLifts)
    {
        if (!tree.IsLiving) return tree;
        float dbhPotential = species.PotentialDbhGrowthCm * site * Stage5Undergrowth.Clamp01(1f - tree.DbhCm / species.MaxDbhCm);
        float dbhGrowth = dbhPotential * (1f / (1f + woodlandCi / species.Ci50));
        float heightGrowth = species.PotentialHeightGrowthM * site * Stage5Undergrowth.Clamp01(1f - tree.HeightM / species.MaxHeightM);
        float dbh = Math.Clamp(tree.DbhCm + dbhGrowth, 1f, 200f);
        float height = Math.Clamp(tree.HeightM + heightGrowth, 0.1f, 60f);
        float radius = Phase2Crown.NextRadiusM(species, tree.CrownRadiusM, dbh, woodlandCi, pruningLifts);
        return tree with { AgeYears = checked(tree.AgeYears + 1), DbhCm = dbh, HeightM = height, CrownRadiusM = radius };
    }
}

/// <summary>CrownModel 1 (C2) for a broadleaf, with the species' own r_pot, Ci50 and k (spec §3.2, owner decision 4).</summary>
internal static class Phase2Crown
{
    /// <summary>r_target = max(0.1, r_pot(DBH) / sqrt(1 + CI/Ci50) · 0.92^lifts); with no lifts the factor is exactly 1.</summary>
    public static float TargetRadiusM(Phase2Species species, float dbhCm, float ci, int pruningLifts = 0) =>
        MathF.Max(Phase2Species.CrownRadiusFloorM,
            species.PotentialCrownRadiusM(dbhCm) / MathF.Sqrt(1f + ci / species.Ci50) * PruningRules.TargetFactor(pruningLifts));

    /// <summary>One annual relaxation step: max(0.1, r + (r_target − r) · k).</summary>
    public static float NextRadiusM(Phase2Species species, float radiusM, float dbhCm, float ci, int pruningLifts) =>
        MathF.Max(Phase2Species.CrownRadiusFloorM,
            radiusM + (TargetRadiusM(species, dbhCm, ci, pruningLifts) - radiusM) * species.CrownRelaxation);
}

/// <summary>
/// Broadleaf juvenile responses (spec §6.2), ported from Unity TreeSpeciesDefinition.InterpolateResponse and
/// JuvenileEcologyRules with the same float32/double order. A browsed fraction of 0 takes the exact light-only path.
/// </summary>
internal static class Phase2Juvenile
{
    /// <summary>Unity InterpolateResponse: clamp the light, InverseLerp within the segment, then Lerp; fallback for a short table.</summary>
    public static float Interpolate(float relativeLight, float[] lights, float[] factors, float fallback)
    {
        if (lights.Length < 2 || factors.Length < 2) return fallback;
        int count = Math.Min(lights.Length, factors.Length);
        float light = Stage5Undergrowth.Clamp01(relativeLight);
        if (light <= lights[0]) return factors[0];
        for (int i = 1; i < count; i++)
        {
            if (light <= lights[i])
            {
                float t = Stage5Juvenile.InverseLerp(lights[i - 1], lights[i], light);
                return Stage5Undergrowth.Lerp(factors[i - 1], factors[i], t);
            }
        }
        return factors[count - 1];
    }

    /// <summary>LR_growth(light): the growth table (Unity JuvenileLightResponse).</summary>
    public static float LightResponse(Phase2Species species, float light) =>
        Interpolate(light, species.GrowthLight, species.GrowthFactor, Stage5Undergrowth.Clamp01(light));

    /// <summary>
    /// Light survival in double (Unity JuvenileEcologyRules.SurvivalResponse): Oak's distinct survival table
    /// (clamp01, fallback 1); Beech's legacy rule 1 − 0.2 when LR(light) &lt; 0.15, else 1.
    /// </summary>
    public static double LightSurvival(Phase2Species species, float light)
    {
        if (species.UsesDistinctJuvenileLightResponses)
            return Stage5Undergrowth.Clamp01(Interpolate(light, species.SurvivalLight, species.SurvivalFactor, 1f));
        return LightResponse(species, light) < species.RegenPoorLightThreshold ? 1d - species.RegenMortalityUnderPoorLight : 1d;
    }

    /// <summary>LR_establish(light): Oak's distinct table (max 0, fallback 1); Beech's shared growth table.</summary>
    public static float EstablishmentResponse(Phase2Species species, float light) =>
        species.UsesDistinctJuvenileLightResponses
            ? MathF.Max(0f, Interpolate(light, species.EstablishmentLight, species.EstablishmentFactor, 1f))
            : LightResponse(species, light);

    /// <summary>V(h): 1 up to 1.2 m, a linear taper to 0 at 1.8 m, then 0.</summary>
    public static float BrowseHeightVulnerability(Phase2Species species, float heightM)
    {
        float full = species.BrowseFullVulnerabilityHeightM;
        float escape = species.BrowseEscapeHeightM;
        if (heightM <= full) return 1f;
        if (heightM >= escape) return 0f;
        return 1f - (heightM - full) / (escape - full);
    }

    /// <summary>
    /// p = min(0.95, pressure · palatability · V(h) · vegetationExposure 1.0 · access), the Unity AssessBrowse product order.
    /// Browse concealment is excluded, so vegetation exposure is fixed at 1.0. Access is 1 for cohorts, 0 or 1 for an exact
    /// planted juvenile (binary shelter access).
    /// </summary>
    public static float BrowseProbability(Phase2Species species, float backgroundPressure, float preGrowthHeightM, float access)
    {
        float effective = Stage5Undergrowth.Clamp01(backgroundPressure) * species.BrowsePalatability *
            BrowseHeightVulnerability(species, preGrowthHeightM) * 1f * Stage5Undergrowth.Clamp01(access);
        return MathF.Min(Stage5Constants.BrowseProbabilityCap, MathF.Max(0f, effective));
    }

    /// <summary>h + 0.35 · LR(light) · site, the increment reduced by 0.90 · p when browsed; p = 0 is the light-only expression.</summary>
    public static float GrowHeight(Phase2Species species, float heightM, float light, float browsedFraction)
    {
        float response = LightResponse(species, light);
        if (browsedFraction <= 0f)
            return heightM + species.RegenHeightGrowthMPerYear * response * Stage5Constants.SiteProductivity;
        float increment = species.RegenHeightGrowthMPerYear * response * Stage5Constants.SiteProductivity;
        return heightM + increment * (1f - Stage5Undergrowth.Clamp01(browsedFraction) * Stage5Constants.BrowseHeightIncrementLoss);
    }

    /// <summary>Light survival times (1 − 0.04 · p) for p &gt; 0; exactly the light value when p is 0.</summary>
    public static double LightBrowseSurvival(Phase2Species species, float light, float browsedFraction)
    {
        double survival = LightSurvival(species, light);
        if (browsedFraction <= 0f) return survival;
        return survival * (1d - Stage5Undergrowth.Clamp01(browsedFraction) * (double)Stage5Constants.BrowseExtraMortality);
    }

    /// <summary>A cohort's whole annual survival: light · (1 − 0.04·p) · (1 − p_veg), in double.</summary>
    public static double Survival(Phase2Species species, float light, float browsedFraction, float vegetationLossProbability)
    {
        double survival = LightBrowseSurvival(species, light, browsedFraction);
        if (vegetationLossProbability <= 0f) return survival;
        return survival * (1d - vegetationLossProbability);
    }

    /// <summary>Unity CanPromote: height ≥ 3.5 m and light ≥ the species' promotionMinimumLight (Oak 0.2, Beech 0).</summary>
    public static bool CanPromote(Phase2Species species, float heightM, float light) =>
        heightM >= species.PromotionHeightM && light >= species.PromotionMinimumLight;

    /// <summary>
    /// The requested natural establishment of one broadleaf in one cell, scaled by a, or 0 (spec §6.3):
    /// seedFactor = 1 − exp(−S/S50); establishment = seedFactor · LR_establish(light) · 1.0; skip when ≤ 0.01;
    /// requested = 0.5 · establishment · a.
    /// </summary>
    public static float RequestedEstablishment(Phase2Species species, float seedRain, float light, Stage5CellScale scale)
    {
        if (seedRain <= 0f) return 0f;
        float seedFactor = 1f - (float)Math.Exp(-seedRain / species.SeedSaturationS50);
        float establishment = seedFactor * EstablishmentResponse(species, light) * Stage5Constants.EstablishmentSuitability;
        if (establishment <= Stage5Constants.MinimumEstablishment) return 0f;
        return establishment * species.RegenDensityPerEstablishment * scale.AreaFactor;
    }
}

/// <summary>The Phase 2 random keys (spec §8). All are new; none is drawn when Species is 0.</summary>
internal static class Phase2Random
{
    public static string MastKey(string speciesId) => "PHASE2-MAST-" + speciesId;
    public static string GenericTreeKey(string treeId) => "ADULT-MORT-GENERIC-" + treeId;
    public static string GenericQuotaKey(string areaId) => "ADULT-MORT-GENERIC-QUOTA-" + areaId;
    public static string JuvenileKey(string juvenileId) => "PHASE2-PJ-" + juvenileId;
    public static string JuvenileVegetationKey(string juvenileId) => JuvenileKey(juvenileId) + "/vegetation";
    public static string JuvenileBrowseKey(string juvenileId) => JuvenileKey(juvenileId) + "/browse";

    /// <summary>The stateless keyed roll (SimulationRandom model 1); drawing one key can never move another key's value.</summary>
    public static float Roll(string key, int year, int seed)
    {
        DrawLog?.Add(key);
        return SimulationRandom.Roll(DerivedManifest.RngModelVersion, key, year, seed);
    }

    /// <summary>Test-only observation seam: when set on a thread, every Phase 2 keyed draw on it appends its key. Null in use.</summary>
    [ThreadStatic] internal static List<string>? DrawLog;
}

/// <summary>
/// One independent annual mast roll per broadleaf (spec §6.1) on its own PHASE2-MAST-{speciesId} stream: r &lt; 0.15 good,
/// r &lt; 0.15 + 0.35 poor, else normal, with the species multipliers. Isolated from STAGE5-MAST and every ADULT-MORT key.
/// </summary>
internal static class Phase2Mast
{
    public static Stage5MastState ForYear(Phase2Species species, int mastSeed, int year) =>
        FromRoll(species, Stage5Random.Stream(Phase2Random.MastKey(species.SpeciesId), mastSeed, year).NextDouble());

    public static Stage5MastState FromRoll(Phase2Species species, double roll)
    {
        if (roll < species.MastGoodProbability) return new(roll, "good", species.MastGoodMultiplier);
        if (roll < species.MastGoodProbability + species.MastPoorProbability) return new(roll, "poor", species.MastPoorMultiplier);
        return new(roll, "normal", species.MastNormalMultiplier);
    }
}

/// <summary>
/// Cross-rack broadleaf seed rain (spec §6.1): S_c,s = Σ_i P_i · exp(−d/λ_s) over living mature adults of species s in every
/// connected rack with d ≤ cutoff_s; P_i = M_s(age) · clamp01(r_i/4) · 1.0 · mast_s. Outer grassland contributes nothing.
/// </summary>
internal static class Phase2SeedRain
{
    /// <summary>M_s(age): 0 at or below onset 40, linear to 1 at 60 (Unity Maturity).</summary>
    public static float Maturity(Phase2Species species, int ageYears)
    {
        if (ageYears <= species.MaturityOnsetYears) return 0f;
        if (ageYears >= species.MaturityFullYears) return 1f;
        return Stage5Juvenile.InverseLerp(species.MaturityOnsetYears, species.MaturityFullYears, ageYears);
    }

    /// <summary>The positive-potential sources of one species, in the order given (the caller passes ordinal TreeId order).</summary>
    public static IReadOnlyList<Stage5SeedSource> Sources(IEnumerable<AnnualTree> trees, Phase2Species species, float mastMultiplier)
    {
        var sources = new List<Stage5SeedSource>();
        foreach (var tree in trees)
        {
            if (!tree.IsLiving) continue;
            float potential = Maturity(species, tree.AgeYears) *
                Stage5Undergrowth.Clamp01(tree.CrownRadiusM / Stage5Constants.SeedCrownReferenceRadiusM) *
                Stage5Constants.SeedPotentialPerMatureTree * mastMultiplier;
            if (potential > 0f) sources.Add(new Stage5SeedSource(tree.XM, tree.ZM, potential));
        }
        return sources.AsReadOnly();
    }

    public static float SeedRainAt(Phase2Species species, float xM, float zM, IReadOnlyList<Stage5SeedSource> sources)
    {
        float seedRain = 0f;
        foreach (var source in sources)
        {
            float dx = xM - source.XM;
            float dz = zM - source.ZM;
            float distance = (float)Math.Sqrt(dx * dx + dz * dz);
            if (distance > species.SeedDispersalCutoffM) continue;
            seedRain += source.Potential * (float)Math.Exp(-distance / species.SeedDispersalScaleM);
        }
        return seedRain;
    }
}

/// <summary>
/// Species-aware pruning (spec §5.2, Unity ForestTree.CanPrune species-aware branch, same guard order). Sitka keeps the
/// unchanged <see cref="PruningRules"/>; Oak and Beech take at most two lifts, three years apart, at exactly 3.5 m then 6.0 m.
/// </summary>
internal static class Phase2Pruning
{
    public static string? RefusalCode(AnnualTree tree, TreePruningState state, float target, int year, Phase2Species? broadleaf)
    {
        if (broadleaf is null) return PruningRules.RefusalCode(tree, state, target, year);
        if (!tree.IsLiving) return "PruneTreeNotLiving";
        if (state.Lifts >= Phase2Constants.BroadleafMaxLifts) return "PruneMaxLiftsReached";
        if (state.LastPruningYear >= 0 && year - state.LastPruningYear < Phase2Constants.BroadleafMinimumYearsBetweenLifts)
            return "PruneIntervalTooShort";
        if (target <= state.CrownBaseHeightM) return "PruneTargetNotAboveCrownBase";
        if (float.IsNaN(target) || float.IsInfinity(target) || target <= 0f || target >= tree.HeightM * PruningRules.MaximumTargetHeightFraction)
            return "PruneTargetOutOfRange";
        if (target != NextLiftTargetM(state.Lifts, broadleaf)) return "PruneBroadleafTargetInvalid";
        return null;
    }

    public static string Message(string code, Phase2Species? broadleaf) => broadleaf is null ? PruningRules.Message(code) : code switch
    {
        "PruneMaxLiftsReached" => "this broadleaf already has the maximum of two high-pruning lifts",
        "PruneIntervalTooShort" => "a broadleaf needs at least three years between pruning lifts",
        "PruneBroadleafTargetInvalid" => "broadleaf high pruning requires the next 3.5 m or 6.0 m lift",
        _ => PruningRules.Message(code),
    };

    public static void Require(AnnualTree tree, TreePruningState state, float target, int year, Phase2Species? broadleaf)
    {
        if (RefusalCode(tree, state, target, year, broadleaf) is string code) throw new P1Refusal(code, Message(code, broadleaf));
    }

    /// <summary>The scheduled target of the next lift: the Sitka schedule, or 3.5 m then 6.0 m for a broadleaf; NaN when none.</summary>
    public static float NextLiftTargetM(int lifts, Phase2Species? broadleaf)
    {
        if (broadleaf is null) return PruningRules.NextLiftTargetM(lifts);
        var targets = Phase2Constants.BroadleafLiftTargetHeightsM;
        return lifts >= 0 && lifts < targets.Count ? targets[lifts] : float.NaN;
    }
}
