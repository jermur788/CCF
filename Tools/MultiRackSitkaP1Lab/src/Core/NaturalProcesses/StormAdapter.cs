using System.Globalization;

namespace SitkaP1.Core.NaturalProcesses;

/// <summary>
/// StormModel adapter. StormModel 0 is storm-free: zero draws, no event, no victims. StormModel 1 (frozen D-050) samples
/// at most ONE woodland-wide event per year from (year, seed) only; lanes never get their own occurrence, severity,
/// direction or multiplier. Victims are chosen in two passes (evaluate everyone, then roll) over trees sorted by ID, so
/// input order cannot change the victim set. Each victim has one outcome: uprooted/fallen ("windthrow"), one fallen
/// deadwood record and one canopy opening. No snapped stems, snags or dominoes.
/// </summary>
public static class StormAdapter
{
    public const string WindthrowCause = "windthrow";
    public const float OpeningIncrementPerVictim = 1f;

    public static StormResult Resolve(StormRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (request.Model == StormModelVersion.StormFree)
            return new StormResult(null, request.Model, true, null, Array.Empty<WindthrowVictim>(), Array.Empty<TreeExposureFeedback>(), 0);
        if (request.Model != StormModelVersion.WindthrowV1)
            return Refused(request.Model, NaturalProcessRefusalCode.UnknownStormModel, NaturalProcessText.RefusalUnknownStormModel);

        NaturalProcessRefusal? refusal = Validate(request);
        if (refusal is not null) return Refused(request.Model, refusal.Code, refusal.Detail);

        StormTreeInput[] trees = request.Trees.OrderBy(t => t.TreeId, StringComparer.Ordinal).ToArray();

        // Pass 1: every index is computed from pre-storm state before any roll or removal.
        float[] index = trees.Select(StormVulnerability.Index).ToArray();
        TreeExposureFeedback[] exposure = trees.Select((t, i) => Feedback(t.TreeId, index[i])).ToArray();

        var rng = new NaturalProcessRandom();
        StormEvent? storm = SampleEvent(rng, request.Year, request.Seed);
        if (storm is null)
            return new StormResult(null, request.Model, false, null, Array.Empty<WindthrowVictim>(), exposure, rng.DrawCount);

        // Pass 2: one ID-keyed roll per tree against the frozen failure chance.
        var victims = new List<WindthrowVictim>();
        for (int i = 0; i < trees.Length; i++)
        {
            float chance = StormVulnerability.FailureChance(index[i], storm.Severity);
            float roll = rng.Roll(NaturalProcessStreams.WindthrowTreePrefix + trees[i].TreeId, request.Year, request.Seed);
            if (roll < chance) victims.Add(Victim(trees[i], request.Year));
        }
        return new StormResult(null, request.Model, false, storm, victims, exposure, rng.DrawCount);
    }

    /// <summary>Qualitative bands for the given trees, sorted by ID. Makes no draws; never exposes the index or p.</summary>
    public static IReadOnlyList<TreeExposureFeedback> ExposureBands(IEnumerable<StormTreeInput> trees) =>
        ContractSnapshot.Of(SnapshotCopy.ToArray(trees).OrderBy(t => t.TreeId, StringComparer.Ordinal)
            .Select(t => Feedback(t.TreeId, StormVulnerability.Index(t))));

    /// <summary>
    /// The UI fall-glyph orientation: equal to the storm's blow-towards bearing. A visual convention for the event
    /// notice only, never a persisted per-tree fall bearing (no per-tree jitter is applied).
    /// </summary>
    public static double FallGlyphOrientation(double directionDegrees) => directionDegrees;

    internal static StormEvent? SampleEvent(NaturalProcessRandom rng, int year, int seed)
    {
        if (rng.Roll(NaturalProcessStreams.StormOccurs, year, seed) >= StormVulnerability.AnnualProbability) return null;
        float[] w = StormVulnerability.Weights;
        float total = w[0] + w[1] + w[2];
        float roll = rng.Roll(NaturalProcessStreams.StormSeverity, year, seed) * total;
        int level = roll < w[0] ? 0 : roll < w[0] + w[1] ? 1 : 2;
        double u = rng.Roll(NaturalProcessStreams.StormDirection, year, seed);
        // Frozen D-050 (StormModel.cs): uniform [0, 360), the bearing the wind blows TOWARDS; 0 = north (+Z), 90 = east (+X).
        double direction = u * 360.0;
        return new StormEvent(year, level, StormVulnerability.Severities[level], direction, FallGlyphOrientation(direction));
    }

    private static TreeExposureFeedback Feedback(string treeId, float index)
    {
        StormExposureBand band = StormVulnerability.Band(index);
        return new TreeExposureFeedback(treeId, band, NaturalProcessText.BandLabel(band));
    }

    private static WindthrowVictim Victim(StormTreeInput tree, int year)
    {
        string deadwoodId = DeadwoodIdFor(tree.TreeId, year);
        var deadwood = new FallenDeadwoodRecord(deadwoodId, tree.TreeId, WindthrowCause, year, DeadwoodForm.Fallen, tree.HeightM, tree.DbhCm);
        return new WindthrowVictim(tree.TreeId, tree.LaneId, WindthrowCause, deadwood,
            new CanopyOpening(tree.CellId, tree.TreeId, OpeningIncrementPerVictim));
    }

    public static string DeadwoodIdFor(string treeId, int year) =>
        "DW-WINDTHROW-" + year.ToString(CultureInfo.InvariantCulture) + "-" + treeId;

    private static NaturalProcessRefusal? Validate(StormRequest request)
    {
        if (request.Year < 1)
            return new(NaturalProcessRefusalCode.InvalidYear, NaturalProcessText.RefusalInvalidYear);
        if (request.LastResolvedStormYear is int last && last >= request.Year)
            return new(NaturalProcessRefusalCode.StormYearAlreadyResolved, NaturalProcessText.RefusalStormYearResolved);
        if (request.Trees is null)
            return new(NaturalProcessRefusalCode.InvalidTreeInput, "tree list is missing");

        var seen = new HashSet<string>(StringComparer.Ordinal);
        foreach (StormTreeInput t in request.Trees)
        {
            if (t is null || string.IsNullOrWhiteSpace(t.TreeId) || t.LaneId is null || t.CellId is null ||
                !float.IsFinite(t.HeightM) || t.HeightM <= 0f || !float.IsFinite(t.DbhCm) || t.DbhCm <= 0f ||
                !float.IsFinite(t.LocalTopHeightM) || t.LocalTopHeightM < 0f ||
                !float.IsFinite(t.CellLight) || t.CellLight < 0f || t.CellLight > 1f ||
                !float.IsFinite(t.RecentOpening) || t.RecentOpening < 0f)
                return new(NaturalProcessRefusalCode.InvalidTreeInput, "invalid storm input for tree " + (t?.TreeId ?? "(null)"));
            if (t.SiteHazard != StormVulnerability.NeutralSiteHazard)
                return new(NaturalProcessRefusalCode.StormSiteHazardNotNeutral, NaturalProcessText.RefusalSiteNotNeutral);
            if (t.EdgeFacing != StormVulnerability.NeutralEdgeFacing)
                return new(NaturalProcessRefusalCode.StormExternalEdgeNotNeutral, NaturalProcessText.RefusalEdgeNotNeutral);
            if (!seen.Add(t.TreeId))
                return new(NaturalProcessRefusalCode.DuplicateTreeId, "duplicate tree ID " + t.TreeId);
        }
        return null;
    }

    private static StormResult Refused(StormModelVersion model, NaturalProcessRefusalCode code, string detail) =>
        new(new NaturalProcessRefusal(code, detail), model, false, null, Array.Empty<WindthrowVictim>(), Array.Empty<TreeExposureFeedback>(), 0);
}
