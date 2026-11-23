namespace SitkaP1.Core;

/// <summary>
/// Site productivity at a position (experiment metres, X east, Z north): a multiplier on adult DBH and height increments.
/// Stage 4 reads it per tree; the default is exactly 1.0 everywhere (no soil map exists, D-095).
/// </summary>
internal delegate float SiteProductivityField(float xM, float zM);

/// <summary>
/// One piece of a growth-age-phase table, mirroring Unity <c>GrowthAgePhase</c>: from <see cref="StartAgeYears"/> on, the
/// annual DBH and height increments are multiplied by the two factors. Piecewise constant, not a curve.
/// </summary>
internal sealed record GrowthAgePhase(float StartAgeYears, float DbhIncrementFactor, float HeightIncrementFactor);

/// <summary>The Unity <c>GrowthAgePhases</c> rules: lookup and validation. No table means factors 1 and 1.</summary>
internal static class GrowthAgePhases
{
    public static bool HasPhases(IReadOnlyList<GrowthAgePhase>? phases) => phases is { Count: > 0 };

    /// <summary>The factors of the phase that <paramref name="ageYears"/> falls in (Unity FactorsAt, same loop).</summary>
    public static (float Dbh, float Height) FactorsAt(IReadOnlyList<GrowthAgePhase>? phases, float ageYears)
    {
        if (!HasPhases(phases)) return (1f, 1f);
        var current = phases![0];
        for (int i = 1; i < phases.Count; i++)
        {
            if (ageYears < phases[i].StartAgeYears) break;
            current = phases[i];
        }
        return (current.DbhIncrementFactor, current.HeightIncrementFactor);
    }

    /// <summary>Unity TryValidate: absent, or the first phase starts at 0, starts strictly increase, factors finite and ≥ 0.</summary>
    public static void RequireValid(IReadOnlyList<GrowthAgePhase>? phases)
    {
        if (!HasPhases(phases)) return;
        if (phases![0].StartAgeYears != 0f)
            throw new P1Refusal("GrowthAgePhasesInvalid", "the first phase must start at age 0");
        for (int i = 0; i < phases.Count; i++)
        {
            var phase = phases[i];
            if (i > 0 && !(phase.StartAgeYears > phases[i - 1].StartAgeYears))
                throw new P1Refusal("GrowthAgePhasesInvalid", "phase start ages must strictly increase");
            if (!float.IsFinite(phase.DbhIncrementFactor) || phase.DbhIncrementFactor < 0f ||
                !float.IsFinite(phase.HeightIncrementFactor) || phase.HeightIncrementFactor < 0f)
                throw new P1Refusal("GrowthAgePhasesInvalid", "increment factors must be finite and not negative");
        }
    }
}

/// <summary>
/// The Stage 4 growth inputs that are not model switches: the site-productivity field and the growth-age-phase table.
/// <see cref="Default"/> is the production Sitka setting: uniform 1.0 site and no age phases (Sitka has none authored).
/// Synthetic non-default values exist only so tests can prove the seams.
/// </summary>
internal sealed record Stage4Options
{
    public SiteProductivityField SiteProductivity { get; init; } = Uniform;
    public IReadOnlyList<GrowthAgePhase>? GrowthAgePhases { get; init; }

    public static float Uniform(float xM, float zM) => 1f;

    public static readonly Stage4Options Default = new();

    internal Stage4Options RequireValid()
    {
        ArgumentNullException.ThrowIfNull(SiteProductivity);
        SitkaP1.Core.GrowthAgePhases.RequireValid(GrowthAgePhases);
        return this;
    }

    /// <summary>Reads the site value at a tree; a non-finite or non-positive value is refused rather than used.</summary>
    internal float SiteAt(AnnualTree tree)
    {
        float site = SiteProductivity(tree.XM, tree.ZM);
        if (!float.IsFinite(site) || site <= 0f)
            throw new P1Refusal("SiteProductivityInvalid", "site productivity must be a finite positive multiplier");
        return site;
    }
}
