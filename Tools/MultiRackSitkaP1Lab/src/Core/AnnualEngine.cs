using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("SitkaP1.Core.Tests")]

namespace SitkaP1.Core;

public sealed record AnnualDeath(string TreeId, int EcologicalYear, string Cause, string Mechanism);

public sealed record AnnualTree(string TreeId, float XM, float ZM, int AgeYears,
    float DbhCm, float HeightM, float CrownRadiusM, AnnualDeath? Death = null)
{
    public bool IsLiving => Death is null;
}

public sealed record AnnualState(int EcologicalYear, IReadOnlyList<AnnualTree> Trees);

public sealed record AnnualStand(int LivingCount, double StemsPerHa, double BasalAreaM2PerHa,
    double QuadraticMeanDbhCm, double RelativeDensity);

public sealed record AnnualTreeAudit(AnnualTree Before, AnnualTree After, int Neighbours, float PreGrowthCi,
    double? Suppression, double? DeathProbability, float? MortalityRoll);

public sealed record AnnualReport(int YearBefore, int YearAfter, AnnualStand Before, AnnualStand PostGrowth,
    AnnualStand After, double PostGrowthDensityPressure, double? MeanVulnerability,
    IReadOnlyList<AnnualTreeAudit> Trees, IReadOnlyList<AnnualDeath> Deaths);

/// <summary>One in-memory annual transaction on a trust-checked 30 m genesis.</summary>
public sealed class AnnualEngine
{
    private readonly object sync = new();
    private readonly float areaHa;
    private readonly WorkSourceIdentity identity;
    private AnnualState state;

    private AnnualEngine(DerivedLoad load)
    {
        var manifest = load.Manifest;
        SimulationSeed = checked((int)manifest.SimulationSeed);
        areaHa = F32.FromBits(load.Record.AreaHaBits);
        // The exact admitted source identity a Work Plan binds to (fixture file bytes and canonical content).
        identity = new WorkSourceIdentity(P1DomainWire.Name(P1Domain.DerivedP1ThirtyMetreExperimentV1),
            manifest.DerivedFixtureId, load.Record.FileSha256, manifest.ContentHash,
            DerivedManifest.RngModelVersion, DerivedManifest.GrowthModelVersion, DerivedManifest.P1AlgorithmVersion,
            manifest.ActionEligibleDbhThresholdMm);
        var trees = manifest.Trees.Select(tree => new AnnualTree(tree.TreeId,
            F32.FromBits(tree.UnityXBits), F32.FromBits(tree.UnityZBits), checked((int)tree.AgeYears),
            F32.FromBits(tree.DbhCmBits), F32.FromBits(tree.HeightMBits), F32.FromBits(tree.CrownRadiusMBits)));
        state = new AnnualState(checked((int)manifest.InitialEcologicalYear), Array.AsReadOnly(trees.ToArray()));
    }

    public static AnnualEngine Open(P1Domain domain, string fixtureId, string fixtureDirectory)
    {
        RequireAnnualDomain(domain, fixtureId);
        return new AnnualEngine(FixtureAccess.ReadDerived(fixtureDirectory, fixtureId, TrustUse.RequireAccepted));
    }

    public static void RequireAnnualDomain(P1Domain domain, string fixtureId)
    {
        _ = P1DomainWire.Name(domain);
        var refusal = AnnualGate.Check(domain, DerivedCatalog.Find(fixtureId));
        // The existing gate still owns domain and catalog refusals. Replace only its engine-availability sentinel.
        if (refusal.Code != "AnnualEngineNotImplemented") throw refusal;
    }

    public int SimulationSeed { get; }

    public AnnualState State
    {
        get { lock (sync) return state; }
    }

    // Private experiment seam: no CLI route or public continuation contract.
    internal AnnualContinuation ContinueInMemory()
    {
        lock (sync) return new AnnualContinuation(state, SimulationSeed, areaHa, identity);
    }

    public AnnualReport Advance() => Advance(beforeCommit: null);

    internal AnnualReport Advance(Action? beforeCommit)
    {
        lock (sync)
        {
            if (state.EcologicalYear != 0)
                throw new P1Refusal("AnnualAlreadyAdvanced", "this experiment has already completed its one ecological year");

            var report = AnnualRules.Calculate(state, SimulationSeed, areaHa);
            var next = new AnnualState(report.YearAfter, Array.AsReadOnly(report.Trees.Select(tree => tree.After).ToArray()));
            beforeCommit?.Invoke();
            state = next;
            return report;
        }
    }
}
