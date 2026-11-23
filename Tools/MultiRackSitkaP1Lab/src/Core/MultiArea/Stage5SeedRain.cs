namespace SitkaP1.Core;

/// <summary>One living seed tree and its seed potential this year.</summary>
internal readonly record struct Stage5SeedSource(float XM, float ZM, float Potential);

/// <summary>
/// Cross-rack Sitka seed rain (spec §3 step 8), ported from Unity ComputeSpeciesSeedRain with the same float32 order:
///   S_c = Σ_i P_i · exp(−d_ic / 20) over living trees with d ≤ 150 m, P_i = M(age) · clamp01(r_i / 4) · 1.0 · mast.
/// The sources are every living tree of the connected woodland (all racks) in ordinal TreeId order, so the sum does not
/// depend on area order. Only real trees seed: the open grassland beyond the outer perimeter contributes nothing.
/// No RNG is used.
/// </summary>
internal static class Stage5SeedRain
{
    /// <summary>M(age): 0 at or below 20 years, (age − 20) / 10 between, 1 at or above 30 (Unity Maturity).</summary>
    public static float Maturity(int ageYears)
    {
        if (ageYears <= Stage5Constants.MaturityOnsetYears) return 0f;
        if (ageYears >= Stage5Constants.MaturityFullYears) return 1f;
        return Stage5Juvenile.InverseLerp(Stage5Constants.MaturityOnsetYears, Stage5Constants.MaturityFullYears, ageYears);
    }

    /// <summary>P_i for one living tree; r_i is its realised CrownModel 1 radius.</summary>
    public static float SeedPotential(AnnualTree tree, float mastMultiplier)
    {
        float maturity = Maturity(tree.AgeYears);
        float crown = Stage5Undergrowth.Clamp01(tree.CrownRadiusM / Stage5Constants.SeedCrownReferenceRadiusM);
        return maturity * crown * Stage5Constants.SeedPotentialPerMatureTree * mastMultiplier;
    }

    /// <summary>The trees with a positive potential, in the order given (the caller passes ordinal TreeId order).</summary>
    public static IReadOnlyList<Stage5SeedSource> Sources(IEnumerable<AnnualTree> woodlandInTreeIdOrder, float mastMultiplier)
    {
        var sources = new List<Stage5SeedSource>();
        foreach (var tree in woodlandInTreeIdOrder)
        {
            if (!tree.IsLiving) continue;
            float potential = SeedPotential(tree, mastMultiplier);
            if (potential > 0f) sources.Add(new Stage5SeedSource(tree.XM, tree.ZM, potential));
        }
        return sources.AsReadOnly();
    }

    /// <summary>Seed rain at one cell centre (Unity Vector2.Distance and Mathf.Exp in float32).</summary>
    public static float SeedRainAt(float xM, float zM, IReadOnlyList<Stage5SeedSource> sources)
    {
        float seedRain = 0f;
        foreach (var source in sources)
        {
            float dx = xM - source.XM;
            float dz = zM - source.ZM;
            float distance = (float)Math.Sqrt(dx * dx + dz * dz);
            if (distance > Stage5Constants.SeedDispersalCutoffM) continue;
            seedRain += source.Potential * (float)Math.Exp(-distance / Stage5Constants.SeedDispersalScaleM);
        }
        return seedRain;
    }
}
