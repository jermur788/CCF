namespace SitkaP1.Core;

internal sealed record ContinuationSnapshot(AnnualState State, int Revision);

/// <summary>In-memory, no-intervention experiment; the one-year public engine remains separate.</summary>
internal sealed class AnnualContinuation
{
    private readonly object sync = new();
    private readonly int simulationSeed;
    private readonly float areaHa;
    private ContinuationSnapshot snapshot;

    internal AnnualContinuation(AnnualState initial, int simulationSeed, float areaHa)
    {
        this.simulationSeed = simulationSeed;
        this.areaHa = areaHa;
        // Own the collection so an external mutable list cannot change a committed snapshot.
        snapshot = new(initial with { Trees = Array.AsReadOnly(initial.Trees.ToArray()) }, 0);
    }

    internal ContinuationSnapshot Snapshot
    {
        get { lock (sync) return snapshot; }
    }

    internal AnnualReport Advance(int expectedYear, int expectedRevision, Action? beforeCommit = null)
    {
        lock (sync)
        {
            var previous = snapshot;
            if (expectedYear != previous.State.EcologicalYear)
                throw new P1Refusal("ContinuationYearMismatch", "expected ecological year is stale");
            if (expectedRevision != previous.Revision)
                throw new P1Refusal("ContinuationRevisionMismatch", "expected revision is stale");
            if (!previous.State.Trees.Any(tree => tree.IsLiving))
                throw new P1Refusal("ContinuationNoLivingStems", "no living stems remain to advance");

            int revision = checked(previous.Revision + 1);
            var report = AnnualRules.Calculate(previous.State, simulationSeed, areaHa);
            var next = new ContinuationSnapshot(new AnnualState(report.YearAfter,
                Array.AsReadOnly(report.Trees.Select(row => row.After).ToArray())), revision);
            // Test-only fault seam: all calculations completed, neither fence nor state published yet.
            beforeCommit?.Invoke();
            snapshot = next;
            return report;
        }
    }
}
