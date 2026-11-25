namespace SitkaP1.Core;

// CCF Multi 2D — Phase 1 Step 1 (spec multi-2d-phase1-time-history-core-v1 §6):
// Pure read-only spatial woodland structure descriptors (Species Mingling M_i and Diameter Differentiation T_i)
// over all living adult trees across the connected simulated woodland. Zero mutation, zero off-map synthetic
// neighbours, never invoked from Advance or any mutation path.

/// <summary>Per-area spatial structure summary grouped by each focal living tree's owning AreaId.</summary>
internal sealed record WoodlandAreaStructureSummary(
    string AreaId,
    double? MeanMingling,
    double? MeanDiameterDifferentiation,
    int FocalLivingTreeCount,
    int ValidDiameterFocalCount,
    int EffectiveNeighbourCount,
    int MaxNeighbourCountK);

/// <summary>Whole-woodland and per-area nearest-neighbour spatial structure descriptors (up to k = 3 living adults).</summary>
internal sealed record WoodlandStructureSummary(
    double? MeanMingling,
    double? MeanDiameterDifferentiation,
    int FocalLivingTreeCount,
    int ValidDiameterFocalCount,
    int EffectiveNeighbourCount,
    int MaxNeighbourCountK,
    IReadOnlyList<WoodlandAreaStructureSummary> Areas,
    HistoryProvenance Provenance,
    string ProvenanceLabel,
    string HonestyLabel);

/// <summary>
/// Deterministic, pure nearest-neighbour Species Mingling (<c>M_i</c>) and Diameter Differentiation (<c>T_i</c>)
/// computation over living adult trees in a committed <see cref="MultiAreaRoot"/>.
/// </summary>
internal static class WoodlandStructureMetrics
{
    internal const int MaxNeighbourCountK = 3;

    internal const string HonestyLabel =
        "Spatial structure descriptor (up to k=3 nearest living trees) — not measured biodiversity, habitat quality, or an Irish forecast.";

    private readonly record struct LivingAdult(int AreaIndex, string AreaId, string TreeId, double XM, double ZM,
        double DbhCm, string SpeciesId);

    private readonly record struct NeighbourCandidate(double DistanceSquared, string TreeId, int Index);

    /// <summary>
    /// Computes Species Mingling (<c>M_i</c>) and Diameter Differentiation (<c>T_i</c>) for every living adult tree
    /// across all areas of <paramref name="root"/>, returning per-area and whole-woodland means. Pure; mutates nothing.
    /// </summary>
    internal static WoodlandStructureSummary Compute(MultiAreaRoot root)
    {
        ArgumentNullException.ThrowIfNull(root);

        int areaCount = root.Areas.Count;
        var living = new List<LivingAdult>();
        var areaLivingCounts = new int[areaCount];

        for (int a = 0; a < areaCount; a++)
        {
            var area = root.Areas[a];
            foreach (var tree in area.State.Trees)
            {
                if (!tree.IsLiving) continue;
                areaLivingCounts[a]++;
                living.Add(new LivingAdult(
                    a,
                    area.AreaId,
                    tree.TreeId,
                    tree.XM,
                    tree.ZM,
                    tree.DbhCm,
                    SpeciesOf(root.Phase2, tree.TreeId)));
            }
        }

        int n = living.Count;
        int k = n >= 2 ? Math.Min(MaxNeighbourCountK, n - 1) : 0;

        if (k == 0)
        {
            var unavailableAreas = new WoodlandAreaStructureSummary[areaCount];
            for (int a = 0; a < areaCount; a++)
            {
                unavailableAreas[a] = new WoodlandAreaStructureSummary(
                    root.Areas[a].AreaId,
                    MeanMingling: null,
                    MeanDiameterDifferentiation: null,
                    FocalLivingTreeCount: areaLivingCounts[a],
                    ValidDiameterFocalCount: 0,
                    EffectiveNeighbourCount: 0,
                    MaxNeighbourCountK: MaxNeighbourCountK);
            }

            return new WoodlandStructureSummary(
                MeanMingling: null,
                MeanDiameterDifferentiation: null,
                FocalLivingTreeCount: n,
                ValidDiameterFocalCount: 0,
                EffectiveNeighbourCount: 0,
                MaxNeighbourCountK: MaxNeighbourCountK,
                Areas: Array.AsReadOnly(unavailableAreas),
                Provenance: HistoryProvenance.Derived,
                ProvenanceLabel: HistoryProvenanceCatalog.LabelOf(HistoryProvenance.Derived),
                HonestyLabel: HonestyLabel);
        }

        var areaMinglingSum = new double[areaCount];
        var areaDiameterSum = new double[areaCount];
        var areaValidDiameterCount = new int[areaCount];

        double totalMinglingSum = 0.0;
        double totalDiameterSum = 0.0;
        int totalValidDiameterCount = 0;

        // Reusable top-k buffer (capacity 1..3), kept strictly sorted by (DistanceSquared asc, TreeId ordinal asc).
        var nearest = new NeighbourCandidate[k];

        for (int i = 0; i < n; i++)
        {
            var focal = living[i];
            int filled = 0;

            for (int j = 0; j < n; j++)
            {
                if (j == i) continue;
                var other = living[j];
                double dx = other.XM - focal.XM;
                double dz = other.ZM - focal.ZM;
                double distSq = dx * dx + dz * dz;
                var candidate = new NeighbourCandidate(distSq, other.TreeId, j);

                if (filled < k)
                {
                    int pos = filled++;
                    while (pos > 0 && CompareCandidate(candidate, nearest[pos - 1]) < 0)
                    {
                        nearest[pos] = nearest[pos - 1];
                        pos--;
                    }
                    nearest[pos] = candidate;
                }
                else if (CompareCandidate(candidate, nearest[k - 1]) < 0)
                {
                    int pos = k - 1;
                    while (pos > 0 && CompareCandidate(candidate, nearest[pos - 1]) < 0)
                    {
                        nearest[pos] = nearest[pos - 1];
                        pos--;
                    }
                    nearest[pos] = candidate;
                }
            }

            int differentSpeciesCount = 0;
            double diameterDiffSum = 0.0;
            int validDiameterPairs = 0;

            for (int r = 0; r < k; r++)
            {
                var neighbour = living[nearest[r].Index];
                if (!string.Equals(focal.SpeciesId, neighbour.SpeciesId, StringComparison.Ordinal))
                    differentSpeciesCount++;

                double di = focal.DbhCm;
                double dj = neighbour.DbhCm;
                if (double.IsFinite(di) && double.IsFinite(dj) && di >= 0.0 && dj >= 0.0)
                {
                    double maxD = Math.Max(di, dj);
                    if (maxD > 0.0)
                    {
                        double minD = Math.Min(di, dj);
                        diameterDiffSum += 1.0 - (minD / maxD);
                        validDiameterPairs++;
                    }
                }
            }

            double minglingI = (double)differentSpeciesCount / k;
            areaMinglingSum[focal.AreaIndex] += minglingI;
            totalMinglingSum += minglingI;

            if (validDiameterPairs > 0)
            {
                double diameterDiffI = diameterDiffSum / validDiameterPairs;
                areaDiameterSum[focal.AreaIndex] += diameterDiffI;
                areaValidDiameterCount[focal.AreaIndex]++;
                totalDiameterSum += diameterDiffI;
                totalValidDiameterCount++;
            }
        }

        var areaSummaries = new WoodlandAreaStructureSummary[areaCount];
        for (int a = 0; a < areaCount; a++)
        {
            int countA = areaLivingCounts[a];
            double? meanM = countA > 0 ? areaMinglingSum[a] / countA : null;
            double? meanT = areaValidDiameterCount[a] > 0 ? areaDiameterSum[a] / areaValidDiameterCount[a] : null;
            areaSummaries[a] = new WoodlandAreaStructureSummary(
                root.Areas[a].AreaId,
                MeanMingling: meanM,
                MeanDiameterDifferentiation: meanT,
                FocalLivingTreeCount: countA,
                ValidDiameterFocalCount: areaValidDiameterCount[a],
                EffectiveNeighbourCount: countA > 0 ? k : 0,
                MaxNeighbourCountK: MaxNeighbourCountK);
        }

        return new WoodlandStructureSummary(
            MeanMingling: totalMinglingSum / n,
            MeanDiameterDifferentiation: totalValidDiameterCount > 0 ? totalDiameterSum / totalValidDiameterCount : null,
            FocalLivingTreeCount: n,
            ValidDiameterFocalCount: totalValidDiameterCount,
            EffectiveNeighbourCount: k,
            MaxNeighbourCountK: MaxNeighbourCountK,
            Areas: Array.AsReadOnly(areaSummaries),
            Provenance: HistoryProvenance.Derived,
            ProvenanceLabel: HistoryProvenanceCatalog.LabelOf(HistoryProvenance.Derived),
            HonestyLabel: HonestyLabel);
    }

    private static string SpeciesOf(MultiAreaPhase2State? phase2, string treeId)
    {
        if (phase2 is not null && phase2.TreeSpecies.TryGetValue(treeId, out var speciesId) &&
            Phase2Species.BroadleafOrNull(speciesId) is not null)
        {
            return speciesId;
        }
        return Phase2Species.SitkaId;
    }

    private static int CompareCandidate(NeighbourCandidate a, NeighbourCandidate b)
    {
        int cmp = a.DistanceSquared.CompareTo(b.DistanceSquared);
        return cmp != 0 ? cmp : string.CompareOrdinal(a.TreeId, b.TreeId);
    }
}

internal sealed partial class MultiAreaContinuation
{
    /// <summary>
    /// Computes read-only spatial structure descriptors (<c>M_i</c> and <c>T_i</c>) over the current committed living
    /// adult woodland. Locks once to snapshot the immutable root; never mutates state.
    /// </summary>
    internal WoodlandStructureSummary ComputeStructure()
    {
        MultiAreaRoot snapshot;
        lock (sync) snapshot = root;
        return WoodlandStructureMetrics.Compute(snapshot);
    }
}
