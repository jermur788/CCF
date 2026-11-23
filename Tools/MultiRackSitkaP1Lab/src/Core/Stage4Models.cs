namespace SitkaP1.Core;

/// <summary>
/// Which adult growth law a multi-area session uses (Multi Stage 4, owner decision D-095). The default everywhere is
/// <see cref="Legacy"/>; model 1 must be selected explicitly together with <see cref="MortalityModelVersion.RecordedDeadwood"/>
/// and <see cref="CrownModelVersion.AreaScaledEquilibriumC2"/>.
/// </summary>
internal enum AdultModelVersion
{
    /// <summary>Model 0: DBH growth uses this area's own Hegyi CI; Sitka height follows the site-class envelope. Frozen.</summary>
    Legacy = 0,

    /// <summary>
    /// Model 1: DBH growth uses the connected-woodland 8 m Hegyi CI (both racks), DBH and height increments are scaled by the
    /// per-position site-productivity seam (neutral 1.0 by default) and by the growth-age-phase seam (inert for Sitka).
    /// </summary>
    ConnectedWoodland = 1,
}

/// <summary>Which natural-mortality bookkeeping a multi-area session uses (Multi Stage 4, D-095). Default <see cref="Legacy"/>.</summary>
internal enum MortalityModelVersion
{
    /// <summary>Model 0: the frozen per-area density mortality with cause "self-thinning" and no deadwood record.</summary>
    Legacy = 0,

    /// <summary>
    /// Model 1: the same per-rack density arithmetic, but each victim takes an explicit biological-death transition (cause
    /// "DensitySuppression", current year) and emits exactly one fallen-deadwood record.
    /// </summary>
    RecordedDeadwood = 1,
}

/// <summary>The life state of one stem, derived from its recorded death. A felled stem is a stump; a natural death is not.</summary>
internal enum LifeState
{
    Living = 0,
    FelledStump = 1,
    BiologicallyDead = 2,
}

/// <summary>The three model switches of one multi-area session, and the only combinations the engine accepts.</summary>
internal sealed record MultiAreaModelSelection(AdultModelVersion Adult, MortalityModelVersion Mortality, CrownModelVersion Crown)
{
    /// <summary>The frozen MA-1 path (all three models 0).</summary>
    public static readonly MultiAreaModelSelection Legacy =
        new(AdultModelVersion.Legacy, MortalityModelVersion.Legacy, CrownModelVersion.LegacyFrozen);

    /// <summary>The Stage 1–3 path: model 0 growth and mortality with the C2 crown response.</summary>
    public static readonly MultiAreaModelSelection Stage3CrownC2 =
        new(AdultModelVersion.Legacy, MortalityModelVersion.Legacy, CrownModelVersion.AreaScaledEquilibriumC2);

    /// <summary>The Stage 4 path: every Stage 4 behaviour needs all three models at 1 (D-095).</summary>
    public static readonly MultiAreaModelSelection Stage4 =
        new(AdultModelVersion.ConnectedWoodland, MortalityModelVersion.RecordedDeadwood, CrownModelVersion.AreaScaledEquilibriumC2);

    public bool IsStage4 => this == Stage4;

    /// <summary>
    /// Refuses any unknown value and any partial Stage 4 combination: the approved spec defines Stage 4 behaviour only with
    /// all three switches at 1, so a mixed selection (for example adult 1 with mortality 0) would be an unspecified model.
    /// </summary>
    public MultiAreaModelSelection RequireSupported()
    {
        CrownModel.RequireKnown(Crown);
        if (!Enum.IsDefined(Adult)) throw new P1Refusal("AdultModelUnknown", "unknown adult model version");
        if (!Enum.IsDefined(Mortality)) throw new P1Refusal("MortalityModelUnknown", "unknown mortality model version");
        if (this == Legacy || this == Stage3CrownC2 || this == Stage4) return this;
        throw new P1Refusal("Stage4ModelIncomplete",
            "Stage 4 needs AdultModelVersion 1, MortalityModelVersion 1 and CrownModelVersion 1 together; otherwise adult and mortality stay 0");
    }
}

/// <summary>Stable cause strings and the life-state reading of a recorded death.</summary>
internal static class LifeStates
{
    /// <summary>The Stage 4 cause of a density (self-thinning) death; mechanism is "hazard" or "boundary" as in model 0.</summary>
    public const string DensitySuppression = "DensitySuppression";

    /// <summary>A felled tree (Work Plan Fell) is a stump; any other recorded death is biological.</summary>
    public static LifeState Of(AnnualTree tree) => tree.Death is null
        ? LifeState.Living
        : string.Equals(tree.Death.Cause, FellCause.Harvest, StringComparison.Ordinal) ? LifeState.FelledStump : LifeState.BiologicallyDead;
}

/// <summary>Evidence labels shown with Stage 4 output (spec §3).</summary>
internal static class Stage4Evidence
{
    public const string SizeDensity = "Comeau et al. 2010 maximum size-density line: UK/British Sitka transfer, not an Irish field calibration";
    public const string MortalityHazard = "Mortality hazard (onset 0.6, strength 0.08, exponent 4, cap 1.0): Gameplay Calibration, not a measured annual mortality rate";
    public const string SyntheticStands = "Synthetic multi-rack stands: Experimental Demonstration, not an Irish field forecast";
    public const string Deadwood = "Fallen deadwood records carry no decay, habitat score or economic value in this stage";
    public const string Pruning = "Pruning changes crown radius only; no timber-quality or price effect";

    public static readonly IReadOnlyList<string> All = Array.AsReadOnly(new[] { SizeDensity, MortalityHazard, SyntheticStands, Deadwood, Pruning });
}
