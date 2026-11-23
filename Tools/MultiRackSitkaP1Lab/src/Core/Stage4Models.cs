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

/// <summary>
/// The model switches of one multi-area session (selection order Adult / Mortality / Crown / Regeneration), and the only
/// combinations the engine accepts. Regeneration defaults to 0, so every three-switch selection is unchanged.
/// </summary>
internal sealed record MultiAreaModelSelection(AdultModelVersion Adult, MortalityModelVersion Mortality, CrownModelVersion Crown,
    MultiAreaRegenerationModelVersion Regeneration = MultiAreaRegenerationModelVersion.Off)
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

    /// <summary>The Stage 5 path (spec multi-stage5-sitka-regeneration-v1): the Stage 4 models plus Regeneration 1.</summary>
    public static readonly MultiAreaModelSelection Stage5 =
        new(AdultModelVersion.ConnectedWoodland, MortalityModelVersion.RecordedDeadwood, CrownModelVersion.AreaScaledEquilibriumC2,
            MultiAreaRegenerationModelVersion.SitkaCohortCompetitionBrowse);

    /// <summary>True when adult, mortality and crown are all 1 (every Stage 4 behaviour), with or without Stage 5.</summary>
    public bool IsStage4 => Adult == AdultModelVersion.ConnectedWoodland && Mortality == MortalityModelVersion.RecordedDeadwood &&
        Crown == CrownModelVersion.AreaScaledEquilibriumC2;

    /// <summary>True for 1/1/1/1: Stage 4 plus Stage 5 natural regeneration.</summary>
    public bool IsStage5 => IsStage4 && Regeneration == MultiAreaRegenerationModelVersion.SitkaCohortCompetitionBrowse;

    /// <summary>
    /// Refuses any unknown value and any partial combination: the approved specs define Stage 4 behaviour only with adult,
    /// mortality and crown all at 1, and Stage 5 regeneration only on top of that (it needs the Stage 4 adult step, the
    /// refreshed 2.5 m light and CrownModel 1). Supported: 0/0/0/0, 0/0/1/0, 1/1/1/0 and 1/1/1/1.
    /// </summary>
    public MultiAreaModelSelection RequireSupported()
    {
        CrownModel.RequireKnown(Crown);
        if (!Enum.IsDefined(Adult)) throw new P1Refusal("AdultModelUnknown", "unknown adult model version");
        if (!Enum.IsDefined(Mortality)) throw new P1Refusal("MortalityModelUnknown", "unknown mortality model version");
        if (!Enum.IsDefined(Regeneration)) throw new P1Refusal("RegenerationModelUnknown", "unknown regeneration model version");
        if (this == Legacy || this == Stage3CrownC2 || this == Stage4 || this == Stage5) return this;
        if (Regeneration != MultiAreaRegenerationModelVersion.Off)
            throw new P1Refusal("Stage5ModelIncomplete",
                "Stage 5 regeneration needs AdultModelVersion 1, MortalityModelVersion 1 and CrownModelVersion 1 together");
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

    /// <summary>
    /// The biological-death transition (Unity ForestTree.ApplyMortality): records cause and year on a living tree. It is
    /// idempotent: a tree that is already dead or felled is returned unchanged, so a second death can never be recorded.
    /// It is not a Fell, so the tree never becomes a stump and yields nothing.
    /// </summary>
    public static AnnualTree ApplyBiologicalDeath(AnnualTree tree, AnnualDeath death)
    {
        if (!tree.IsLiving) return tree;
        if (string.IsNullOrWhiteSpace(death.Cause) || death.EcologicalYear < 0 ||
            string.Equals(death.Cause, FellCause.Harvest, StringComparison.Ordinal) ||
            !string.Equals(death.TreeId, tree.TreeId, StringComparison.Ordinal))
            throw new P1Refusal("BiologicalDeathInvalid", "a biological death needs this tree's ID, a non-fell cause and a non-negative year");
        return tree with { Death = death };
    }
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
