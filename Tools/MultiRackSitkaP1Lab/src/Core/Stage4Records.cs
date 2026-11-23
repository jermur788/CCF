namespace SitkaP1.Core;

/// <summary>
/// Pruning history of one tree (Unity ForestTree: pruningLifts, crownBaseHeightM, lastPruningYear = -1 when never pruned).
/// Session-only Stage 4 state; it never enters the model 0 tree record or its fingerprint.
/// </summary>
internal sealed record TreePruningState(int Lifts, float CrownBaseHeightM, int LastPruningYear)
{
    public static readonly TreePruningState Unpruned = new(0, 0f, -1);
}

/// <summary>
/// The verified Unity pruning rules for Sitka (ForestTree.CanPrune/TryPrune, model 0/1 legacy path). Gameplay calibration:
/// three lifts at most, five years apart, each removing 8 % of the current crown radius; the target crown base must exceed
/// the current one and stay positive and below 60 % of height. No timber-quality or price effect.
/// </summary>
internal static class PruningRules
{
    public const int MaxLifts = 3;
    public const float CrownRadiusReductionPerLift = 0.08f;
    public const int MinimumYearsBetweenLifts = 5;
    public const float MaximumTargetHeightFraction = 0.6f;

    /// <summary>Unity PrunedCrownTargetFactor: 1 with no lifts, otherwise Mathf.Pow(0.92f, lifts).</summary>
    public static float TargetFactor(int lifts) =>
        lifts == 0 ? 1f : (float)Math.Pow(1f - CrownRadiusReductionPerLift, lifts);

    /// <summary>The immediate cut of one lift: max(0.1, r * 0.92).</summary>
    public static float CutRadiusM(float radiusM) => MathF.Max(0.1f, radiusM * (1f - CrownRadiusReductionPerLift));

    /// <summary>
    /// The first failed guard as a stable refusal code, in Unity's check order, or null when the lift is allowed.
    /// </summary>
    public static string? RefusalCode(AnnualTree tree, TreePruningState state, float target, int year)
    {
        if (!tree.IsLiving) return "PruneTreeNotLiving";
        if (state.Lifts >= MaxLifts) return "PruneMaxLiftsReached";
        if (state.LastPruningYear >= 0 && year - state.LastPruningYear < MinimumYearsBetweenLifts) return "PruneIntervalTooShort";
        if (target <= state.CrownBaseHeightM) return "PruneTargetNotAboveCrownBase";
        if (float.IsNaN(target) || float.IsInfinity(target) || target <= 0f || target >= tree.HeightM * MaximumTargetHeightFraction)
            return "PruneTargetOutOfRange";
        return null;
    }

    public static string Message(string code) => code switch
    {
        "PruneTreeNotLiving" => "only living trees can be pruned",
        "PruneMaxLiftsReached" => "this tree already has the maximum of three pruning lifts",
        "PruneIntervalTooShort" => "a tree needs at least five years between pruning lifts",
        "PruneTargetNotAboveCrownBase" => "the target crown base must exceed the current pruned height",
        "PruneTargetOutOfRange" => "the target crown base must be positive and below 60 % of tree height",
        _ => code,
    };

    public static void Require(AnnualTree tree, TreePruningState state, float target, int year)
    {
        if (RefusalCode(tree, state, target, year) is string code) throw new P1Refusal(code, Message(code));
    }

    /// <summary>
    /// The verified Unity Sitka lift schedule (ScenarioOne.asset pruningLiftTargetHeightsM, read through
    /// ScenarioOneDefinition.NextPruningTargetHeightM): the target crown-base height of lift 1, 2 and 3. Gameplay
    /// calibration, not timber-quality evidence. The /multi player never chooses or sends a target; the engine uses this.
    /// </summary>
    public static readonly IReadOnlyList<float> SitkaLiftTargetHeightsM = Array.AsReadOnly(new[] { 2.5f, 5f, 6.5f });

    /// <summary>The scheduled target of the next lift after <paramref name="lifts"/> lifts, or NaN when no lift is scheduled.</summary>
    public static float NextLiftTargetM(int lifts) =>
        lifts >= 0 && lifts < SitkaLiftTargetHeightsM.Count ? SitkaLiftTargetHeightsM[lifts] : float.NaN;

    /// <summary>The first failed guard for the next scheduled lift in <paramref name="year"/>, or null when it is allowed.</summary>
    public static string? ScheduledRefusalCode(AnnualTree tree, TreePruningState state, int year) =>
        RefusalCode(tree, state, NextLiftTargetM(state.Lifts), year);
}

/// <summary>
/// One fallen-deadwood record, emitted exactly once per biological death (Unity ScenarioOneManager.OnTreeBiologicalDeath).
/// DBH and height are the stem's values at death; the volume is Unity's biological stem volume
/// DBH² × 0.00007854 × (height × formHeightRatio 0.5) in float32. No decay, habitat score or economic value.
/// </summary>
internal sealed record FallenDeadwoodRecord(string DeadwoodId, string TreeId, string AreaId, string SpeciesId, float XM, float ZM,
    int DeathYear, string Cause, float DbhCm, float HeightM, float EstimatedVolumeM3)
{
    public const float SitkaFormHeightRatio = 0.5f;

    /// <summary>Unity BiologicalStemVolumeM3, same float32 expression order.</summary>
    public static float StemVolumeM3(float dbhCm, float heightM) => dbhCm * dbhCm * 0.00007854f * (heightM * SitkaFormHeightRatio);

    /// <summary>Unity numbering: "DW" + a session counter starting at 1, four digits.</summary>
    public static string IdFor(int number) => "DW" + number.ToString("0000", System.Globalization.CultureInfo.InvariantCulture);
}
