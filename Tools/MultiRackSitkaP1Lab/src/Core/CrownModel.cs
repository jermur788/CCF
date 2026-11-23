namespace SitkaP1.Core;

/// <summary>
/// Which crown initialisation and annual crown response a session uses (Multi #899, owner decision D-094).
/// The default everywhere is <see cref="LegacyFrozen"/>; model 1 must be selected explicitly.
/// </summary>
public enum CrownModelVersion
{
    /// <summary>Model 0: year-0 radius = r_pot (CI = 0); annual target = r_pot / (1 + CI/Ci50); k = 0.15. Frozen, byte-identical.</summary>
    LegacyFrozen = 0,

    /// <summary>
    /// Model 1 (C2): r_target = max(0.1, r_pot / sqrt(1 + CI/Ci50)); year-0 r_0 = r_target(CI_0); annual
    /// r_new = max(0.1, r + (r_target - r) * k). CI is Hegyi over the connected woodland. Simulation abstraction /
    /// gameplay calibration, not a measured Irish growth model.
    /// </summary>
    AreaScaledEquilibriumC2 = 1,
}

/// <summary>
/// The C2 crown equations and the connected-woodland Hegyi neighbourhood they use. Pure functions in float32, the same
/// precision as the frozen rules. Model 0 does not use anything here: its path in <see cref="AnnualRules"/> is unchanged.
/// </summary>
public static class CrownModel
{
    public const string LegacyRuleId = DerivedManifest.CrownRuleId;
    public const string C2RuleId = "area-scaled-equilibrium-c2-v1";

    public static string RuleId(CrownModelVersion model) => model switch
    {
        CrownModelVersion.LegacyFrozen => LegacyRuleId,
        CrownModelVersion.AreaScaledEquilibriumC2 => C2RuleId,
        _ => throw new P1Refusal("CrownModelUnknown", "unknown crown model version"),
    };

    public static void RequireKnown(CrownModelVersion model) => _ = RuleId(model);

    /// <summary>Realised C2 target radius in metres: max(floor, r_pot(DBH) / sqrt(1 + CI / Ci50)).</summary>
    public static float TargetRadiusM(SpeciesProfile species, float dbhCm, float ci) =>
        MathF.Max(species.CrownRadiusFloorM, species.PotentialCrownRadiusM(dbhCm) / MathF.Sqrt(1f + ci / species.Ci50));

    /// <summary>One annual C2 relaxation step: max(floor, r + (r_target - r) * k).</summary>
    public static float NextRadiusM(SpeciesProfile species, float radiusM, float dbhCm, float ci) =>
        MathF.Max(species.CrownRadiusFloorM, radiusM + (TargetRadiusM(species, dbhCm, ci) - radiusM) * species.CrownRelaxation);

    /// <summary>
    /// The connected-woodland neighbourhood: every LIVING tree of every area, in ordinal TreeId order. This fixed order makes
    /// the floating-point Hegyi sum independent of area order. Only real trees are included; nothing is added beyond the
    /// outer perimeter (it borders open grassland).
    /// </summary>
    public static AnnualTree[] Woodland(IEnumerable<AnnualTree> trees) =>
        trees.Where(tree => tree.IsLiving).OrderBy(tree => tree.TreeId, StringComparer.Ordinal).ToArray();

    /// <summary>Hegyi CI and neighbour count of <paramref name="target"/> over the woodland (8 m, 0.5 m floor, self-excluded).</summary>
    public static (float Ci, int Neighbours) WoodlandCompetition(AnnualTree target, AnnualTree[] woodland) =>
        AnnualRules.Competition(target, woodland);

    /// <summary>
    /// Year-0 equilibrium: every living tree of <paramref name="areaTrees"/> gets r_0 = r_target(CI_0), with CI_0 evaluated
    /// once on the genesis woodland. Positions, IDs, sizes and states are unchanged; dead trees are returned as they are.
    /// </summary>
    public static IReadOnlyList<AnnualTree> Equilibrate(SpeciesProfile species, IReadOnlyList<AnnualTree> areaTrees, AnnualTree[] woodland) =>
        Array.AsReadOnly(areaTrees.Select(tree => tree.IsLiving
            ? tree with { CrownRadiusM = TargetRadiusM(species, tree.DbhCm, WoodlandCompetition(tree, woodland).Ci) }
            : tree).ToArray());
}
