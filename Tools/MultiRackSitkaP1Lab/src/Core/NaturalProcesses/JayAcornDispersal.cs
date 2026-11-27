using System.Globalization;

namespace SitkaP1.Core.NaturalProcesses;

/// <summary>
/// Jay Acorn Dispersal (OFF by default). In a year the existing engine already calls an oak mast year, up to 10 candidate
/// acorns per hectare (nearest integer) are DIVERTED from the existing on-map oak seed candidates and carried by jays.
/// Diverted candidates must be removed from the caller's ordinary list, so nothing is double counted. Optionally,
/// Nearby Oak Woodland adds up to 6 external candidates per hectare from one outer edge.
///
/// No microsite bias, no browse immunity, no instant seedlings: every delivered acorn is a candidate for the caller's
/// ordinary establishment rules. When OFF, or in a non-mast year, the engine makes zero draws and returns empty results.
/// </summary>
public static class JayAcornDispersal
{
    /// <summary>Gameplay calibration: on-map jay-routed candidates per woodland hectare per mast year.</summary>
    public const double OnMapCandidatesPerHectare = 10.0;

    public const string OakSpeciesId = "sessile-oak";

    /// <summary>Deterministic nearest-integer cap (midpoint away from zero): 10 × 0.64 ha → 6; 6 × 0.64 ha → 4.</summary>
    public static int CapFor(double candidatesPerHectare, double hectares) =>
        (int)Math.Round(candidatesPerHectare * hectares, MidpointRounding.AwayFromZero);

    public static JayDispersalResult Disperse(JayDispersalRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        JayAcornDispersalSettings settings = request.Settings ?? JayAcornDispersalSettings.Default;

        if (!settings.JayAcornDispersal)
        {
            return settings.NearbyOakWoodland
                ? Refused(NaturalProcessRefusalCode.NearbyOakWoodlandRequiresJayDispersal, NaturalProcessText.RefusalNearbyNeedsJay)
                : Empty(ran: false, 0.0, 0, 0);
        }
        if (request.OakMast is null || !request.OakMast.IsOakMastYear)
            return Empty(ran: false, 0.0, 0, 0);

        NaturalProcessRefusal? refusal = Validate(request, settings);
        if (refusal is not null) return Refused(refusal.Code, refusal.Detail);

        double hectares = request.Woodland.AreaHectares;
        int onMapCap = CapFor(OnMapCandidatesPerHectare, hectares);
        int offMapCap = settings.NearbyOakWoodland ? CapFor(NearbyOakWoodland.CandidatesPerHectare, hectares) : 0;

        var rng = new NaturalProcessRandom();
        IReadOnlyList<OnMapOakSeedCandidate> diverted = SelectDiverted(rng, request, onMapCap);
        Dictionary<string, int> indexInSource = IndexWithinSource(request.OnMapOakCandidates);

        var carried = new List<JayAcornCandidate>(diverted.Count + offMapCap);
        foreach (OnMapOakSeedCandidate candidate in diverted)
        {
            carried.Add(JayAcornTransport.Carry(rng, request.Year, request.Seed, candidate.SourceTreeId,
                indexInSource[candidate.CandidateId], candidate.CandidateId, JayCandidateOrigin.OnMapOakTree,
                candidate.SourceTreeId, sourceEdge: null, candidate.SourcePosition, request.Woodland));
        }
        if (offMapCap > 0)
        {
            carried.AddRange(NearbyOakWoodland.Offer(rng, request.Year, request.Seed, settings.NearbyOakWoodlandEdge,
                offMapCap, request.Woodland));
        }

        var delivered = carried.Where(c => c.Fate == JayCandidateFate.DeliveredInsideWoodland)
            .OrderBy(c => c.CandidateId, StringComparer.Ordinal).ToArray();
        var exported = carried.Where(c => c.Fate == JayCandidateFate.ExportedOutsideWoodland)
            .OrderBy(c => c.CandidateId, StringComparer.Ordinal).ToArray();
        var divertedIds = diverted.Select(c => c.CandidateId).OrderBy(id => id, StringComparer.Ordinal).ToArray();

        return new JayDispersalResult(null, true, hectares, onMapCap, offMapCap, divertedIds, delivered, exported, rng.DrawCount);
    }

    /// <summary>
    /// All supplied candidates when at or below the cap (no draws). Above the cap, one keyed draw per candidate and the
    /// lowest rolls win (ties by candidate ID), so the chosen set never depends on input order.
    /// </summary>
    private static IReadOnlyList<OnMapOakSeedCandidate> SelectDiverted(NaturalProcessRandom rng, JayDispersalRequest request, int cap)
    {
        var supplied = request.OnMapOakCandidates.OrderBy(c => c.CandidateId, StringComparer.Ordinal).ToArray();
        if (supplied.Length <= cap) return supplied;
        return supplied
            .Select(c => (Candidate: c, Roll: rng.Roll(NaturalProcessStreams.JayDivertPrefix + c.CandidateId, request.Year, request.Seed)))
            .OrderBy(x => x.Roll)
            .ThenBy(x => x.Candidate.CandidateId, StringComparer.Ordinal)
            .Take(cap)
            .Select(x => x.Candidate)
            .OrderBy(c => c.CandidateId, StringComparer.Ordinal)
            .ToArray();
    }

    /// <summary>Stable per-source index: rank of each candidate among all supplied candidates of the same tree.</summary>
    private static Dictionary<string, int> IndexWithinSource(IReadOnlyList<OnMapOakSeedCandidate> candidates)
    {
        var index = new Dictionary<string, int>(StringComparer.Ordinal);
        foreach (var group in candidates.GroupBy(c => c.SourceTreeId, StringComparer.Ordinal))
        {
            int i = 0;
            foreach (var c in group.OrderBy(c => c.CandidateId, StringComparer.Ordinal))
                index[c.CandidateId] = i++;
        }
        return index;
    }

    private static NaturalProcessRefusal? Validate(JayDispersalRequest request, JayAcornDispersalSettings settings)
    {
        if (request.Year < 1)
            return new(NaturalProcessRefusalCode.InvalidYear, NaturalProcessText.RefusalInvalidYear);
        if (request.Woodland is null || !request.Woodland.IsValid)
            return new(NaturalProcessRefusalCode.InvalidWoodlandBoundary, NaturalProcessText.RefusalInvalidWoodland);
        if (!Enum.IsDefined(settings.NearbyOakWoodlandEdge))
            return new(NaturalProcessRefusalCode.InvalidOuterEdge, NaturalProcessText.RefusalInvalidEdge);
        if (request.OnMapOakCandidates is null)
            return new(NaturalProcessRefusalCode.InvalidCandidate, "on-map oak candidate list is missing");

        var seen = new HashSet<string>(StringComparer.Ordinal);
        foreach (OnMapOakSeedCandidate c in request.OnMapOakCandidates)
        {
            if (c is null || string.IsNullOrWhiteSpace(c.CandidateId) || string.IsNullOrWhiteSpace(c.SourceTreeId) ||
                !double.IsFinite(c.SourcePosition.X) || !double.IsFinite(c.SourcePosition.Y))
                return new(NaturalProcessRefusalCode.InvalidCandidate, "candidate needs an ID, a source tree ID and a finite position");
            if (!string.Equals(c.SpeciesId, OakSpeciesId, StringComparison.Ordinal))
                return new(NaturalProcessRefusalCode.NonOakSeedCandidate,
                    string.Create(CultureInfo.InvariantCulture, $"candidate {c.CandidateId} is {c.SpeciesId}, not {OakSpeciesId}"));
            if (!seen.Add(c.CandidateId))
                return new(NaturalProcessRefusalCode.DuplicateCandidateId, "duplicate candidate ID " + c.CandidateId);
        }
        return null;
    }

    private static JayDispersalResult Empty(bool ran, double hectares, int onMapCap, int offMapCap) =>
        new(null, ran, hectares, onMapCap, offMapCap, Array.Empty<string>(), Array.Empty<JayAcornCandidate>(),
            Array.Empty<JayAcornCandidate>(), 0);

    private static JayDispersalResult Refused(NaturalProcessRefusalCode code, string detail) =>
        new(new NaturalProcessRefusal(code, detail), false, 0.0, 0, 0, Array.Empty<string>(),
            Array.Empty<JayAcornCandidate>(), Array.Empty<JayAcornCandidate>(), 0);
}
