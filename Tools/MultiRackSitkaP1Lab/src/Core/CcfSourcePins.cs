namespace SitkaP1.Core;

/// <summary>
/// Read-only provenance of the CCF Scenario One source that P1 reuses or cites. Values were read from Git at
/// <see cref="InspectedCommit"/> (git blob id, and SHA-256 of the blob bytes). They are provenance, not a parity claim.
/// A test re-reads the blobs and fails if the source changed underneath P1.
/// </summary>
public static class CcfSourcePins
{
    public const string InspectedCommit = "90583811c4fb88508a4bf05c14365732e24a21ac";

    public sealed record Pin(string Path, string Role, string GitBlob, string Sha256);

    public static readonly IReadOnlyList<Pin> Files = new[]
    {
        new Pin("Assets/ForestPrototype/SimulationRandom.cs", "linked-unchanged",
            "bb83d7f8468002ab13a6aec6caf548e0bf3ad34b", "d1b52a604d2593c833e80f4c20bce1ead56f2ad51f19ce12625e4a6bb2ec927b"),
        new Pin("Assets/ForestPrototype/SitkaGrowthModel.cs", "linked-unchanged",
            "e4c73402d46bb3b75a9cba2e4622857a913fac92", "291601041876d83451259990f82219c6ccd3fb704ba55e6dcb8189f13b446463"),
        new Pin("Assets/ForestPrototype/Species/SitkaSpruce.asset", "cited-parameters",
            "e638e28ede09afc201d9e01fed938184b7b282f8", "a6634c76644e5fec31d728f1edbdd6cd2b23e85281d4dced288980eda6251563"),
        new Pin("Assets/ForestPrototype/TreeSpeciesDefinition.cs", "cited-crown-potential",
            "51faaa0de0d218886935509ff64cb4f40598dd03", "0206b64be5c18dee1fc7db8b8a399b92d3d5915be5e31965646ff7dfc784ba48"),
    };

    // Sitka asset values cited by the crown initialisation (SitkaSpruce.asset lines 22–24; usePowerLawCrown: 0 selects the
    // linear branch of TreeSpeciesDefinition.PotentialCrownRadiusM, lines 246–255).
    public const float CrownRadiusIntercept = 0.9415f;
    public const float CrownRadiusPerCmDbh = 0.07635f;
    public const float CrownRadiusFloorM = 0.1f;
}
