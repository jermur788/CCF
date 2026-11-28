using System.Collections.ObjectModel;
using System.Globalization;

namespace SitkaP1.Core;

/// <summary>Per-lane stem and species summary returned alongside a generated forest-maker stand.</summary>
internal sealed record ForestMakerLaneSummary(
    string AreaId,
    string Label,
    float AreaHa,
    int TotalStems,
    int SitkaCount,
    int OakCount,
    int BeechCount,
    int ScotsPineCount = 0);

/// <summary>Complete output of <see cref="ForestMakerGenerator.Generate"/>.</summary>
internal sealed record ForestMakerResult(
    ForestMakerRecipe Recipe,
    string RecipeHash,
    string ContentHash,
    MultiAreaExperiment Experiment,
    Phase2Options Options,
    int TotalStems,
    double TotalAreaHa,
    IReadOnlyList<ForestMakerLaneSummary> Lanes,
    IReadOnlyList<string>? ScotsPineTreeIds = null)
{
    /// <summary>True for a recipe whose site is the Dry Ridge &amp; Moist Slope two-zone map.</summary>
    internal bool IsTwoZone => Recipe.SpeciesMix == ForestMakerSpeciesMix.ScotsPineSitkaTwoZone;

    /// <summary>
    /// The Scots pine / site-moisture genesis of this stand: the two-zone recipe always carries its own site and genesis pine;
    /// any other recipe carries a whole-woodland class only when one other than NeutralLegacy was chosen (else null).
    /// </summary>
    internal ScotsPineGenesis? ScotsPineGenesisFor(SiteMoistureSetting? chosen)
    {
        if (IsTwoZone)
        {
            if (chosen is not null && !chosen.IsNeutral && chosen != SiteMoistureSetting.DryRidgeMoistSlope)
                throw new P1Refusal("SiteMoistureFixedByPreset",
                    "the Dry Ridge & Moist Slope woodland has its own two-zone site; a whole-woodland class cannot replace it");
            return new ScotsPineGenesis(SiteMoistureSetting.DryRidgeMoistSlope, ScotsPineTreeIds ?? Array.Empty<string>());
        }
        if (chosen is null || chosen.IsNeutral) return null;
        if (chosen.Mode != SiteMoistureMode.Whole)
            throw new P1Refusal("SiteMoistureInvalid", "only the Dry Ridge & Moist Slope woodland uses the two-zone site");
        return new ScotsPineGenesis(chosen.RequireValid(), Array.Empty<string>());
    }
}

/// <summary>
/// Deterministic, integer-only rectangular forest maker (SPEC-multi-2d-phase2a-forest-maker-V1 §5.2–§5.3).
/// Produces an ordinary validated <see cref="MultiAreaExperiment"/> and <see cref="Phase2Options"/> that run unchanged
/// on <see cref="MultiAreaContinuation"/>. Randomness uses integer SplitMix64 seeded from <see cref="ForestMakerRecipe.Seed"/>
/// and is independent of the engine's mortality RNG; its namespace prefix is registered in <see cref="RngSalts.ForestMaker"/>.
/// </summary>
internal static class ForestMakerGenerator
{
    // Accepted default Q1 (spec §5.3): species-neutral piecewise-linear mean DBH (mm) by cohort age.
    // Anchors at 20 (Enlarged80) and 47 (MA-1 Area B) are TRANSFER; other anchors are GAMEPLAY CALIBRATION — RESEARCH REQUIRED.
    private static readonly (int Age, long MeanDbhMm)[] CohortMeanDbhAnchors =
    {
        (10, 70L),
        (20, 160L),
        (30, 240L),
        (40, 300L),
        (47, 345L),
        (60, 400L),
    };

    // Accepted default Q2 (spec §5.3): MA-1 Area B older cohort parameters reused for age-60 edge broadleaves.
    private const long EdgeBroadleafMeanDbhMm = 345L;
    private const long EdgeBroadleafSpreadPer1000 = 80L;
    private const long EdgeBroadleafMinDbhMm = 220L;
    private const long EdgeBroadleafMaxDbhMm = 480L;
    private const long EdgeBroadleafHeightAtMeanMm = 27_200L;
    private const long EdgeBroadleafHeightSlopePerDbhMm = 20L;

    private const long CohortMinHeightMm = 3_500L;
    private const long CohortHeightInterceptMm = 2_000L;
    private const long CohortHeightPerDbhMm = 62L;

    internal sealed record LaneBounds(int Index, string AreaId, string Label, long X0Mm, long X1Mm, long LengthMm,
        IReadOnlyList<MillimetrePoint> Polygon)
    {
        internal long WidthMm => X1Mm - X0Mm;
    }

    internal sealed record PlacedStem(long XMm, long YMm, int AgeYears, long DbhMm, long HeightMm, string SpeciesId);

    internal sealed record LatticeCandidate(long XMm, long YMm, int CellIndex, ulong SelectionKey);

    /// <summary>Registered namespace key helper for Forest Maker deterministic streams (references <see cref="RngSalts.ForestMaker"/>).</summary>
    internal static string StreamNamespaceKey(string label) => RngSalts.ForestMaker + label;

    /// <summary>Generates a validated deterministic multi-area woodland from <paramref name="recipe"/>.</summary>
    internal static ForestMakerResult Generate(ForestMakerRecipe recipe)
    {
        ArgumentNullException.ThrowIfNull(recipe);
        recipe.RequireValid();

        string recipeHash = recipe.RecipeHash();
        string experimentId = ForestMakerPresets.ExperimentIdFor(recipe, recipeHash);

        int totalStems = recipe.TotalStems();
        LaneBounds[] lanes = LaneRectangles(recipe.WidthMm, recipe.LengthMm, recipe.LaneCount);
        int[] quotas = ComputeLaneQuotas(totalStems, recipe.WidthMm, recipe.LaneCount);
        RequireNonEmptyQuotas(quotas);
        int[] laneSeeds = LaneSeeds(recipe.Seed, recipe.LaneCount);

        PlacedStem[][] edgeBroadleavesByLane = EdgeBroadleaves(recipe, lanes);
        PlacedStem[] allEdgeBroadleaves = edgeBroadleavesByLane.SelectMany(list => list).ToArray();

        var areas = new MultiAreaDefinition[recipe.LaneCount];
        var laneSummaries = new ForestMakerLaneSummary[recipe.LaneCount];
        var genesisSpecies = new Dictionary<string, string>(StringComparer.Ordinal);
        var scotsPineIds = new List<string>();

        for (int i = 0; i < recipe.LaneCount; i++)
        {
            LaneBounds lane = lanes[i];
            PlacedStem[] edgeBroadleaves = edgeBroadleavesByLane[i];
            int sitkaNeeded = quotas[i] - edgeBroadleaves.Length;
            if (sitkaNeeded < 0)
            {
                throw new P1Refusal("ForestMakerInfeasibleStocking",
                    $"{lane.AreaId} quota ({quotas[i]}) is smaller than required edge broadleaves ({edgeBroadleaves.Length})");
            }

            IReadOnlyList<LatticeCandidate> selected = SelectExact(recipe.Seed, lane, quotas[i], sitkaNeeded, allEdgeBroadleaves);
            var stems = new List<PlacedStem>(quotas[i]);
            stems.AddRange(edgeBroadleaves);

            foreach (var candidate in selected)
            {
                string speciesId = PickCohortSpecies(recipe, i, candidate.CellIndex, candidate.XMm);
                var (dbhMm, heightMm) = CohortStartingSize(recipe.Seed, i, candidate.CellIndex, recipe.StartAgeYears);
                stems.Add(new PlacedStem(candidate.XMm, candidate.YMm, recipe.StartAgeYears, dbhMm, heightMm, speciesId));
            }

            var (trees, sitkaCount, oakCount, beechCount, pineCount) = AssignIds(lane.AreaId, stems, genesisSpecies, scotsPineIds);
            string stockingNote = BuildStockingNote(recipe, lane, sitkaCount, oakCount, beechCount, pineCount);

            var areaDef = new MultiAreaDefinition(lane.AreaId, lane.Label, stockingNote, lane.Polygon, laneSeeds[i], trees);
            areas[i] = areaDef;
            laneSummaries[i] = new ForestMakerLaneSummary(lane.AreaId, lane.Label, areaDef.AreaHa, trees.Count, sitkaCount, oakCount, beechCount,
                pineCount);
        }

        double totalAreaHa = areas.Sum(a => (double)a.AreaHa);
        var experiment = new MultiAreaExperiment(
            experimentId,
            recipe.GeneratorId,
            recipe.GeneratorVersion,
            recipe.Seed,
            Array.AsReadOnly(areas))
        {
            DisclosureOverride = ForestMakerBounds.Disclosure,
            GeneratorNoteOverride = ForestMakerBounds.GeneratorNote,
            BadgeOverride = ForestMakerPresets.BadgeFor(recipe, totalAreaHa),
        };
        experiment.Validate();

        var options = (Phase2Options.Default with
        {
            GenesisSpecies = new ReadOnlyDictionary<string, string>(genesisSpecies),
        }).RequireValid();

        string contentHash = experiment.ContentHash();

        return new ForestMakerResult(
            recipe,
            recipeHash,
            contentHash,
            experiment,
            options,
            totalStems,
            totalAreaHa,
            Array.AsReadOnly(laneSummaries),
            scotsPineIds.Count == 0 ? null : Array.AsReadOnly(scotsPineIds.Order(StringComparer.Ordinal).ToArray()));
    }

    /// <summary>
    /// Convenience entry helper to open a <see cref="MultiAreaContinuation"/> from a validated recipe. <paramref name="siteMoisture"/>
    /// is the New Woodland whole-woodland class; null or NeutralLegacy keeps every existing woodland exactly as before.
    /// </summary>
    internal static (ForestMakerResult Generated, MultiAreaContinuation Session) CreateSession(
        ForestMakerRecipe recipe,
        MultiAreaModelSelection? models = null,
        Stage4Options? stage4 = null,
        Stage5Options? stage5 = null,
        EconomyOptions? economy = null,
        SiteMoistureSetting? siteMoisture = null)
    {
        var generated = Generate(recipe);
        var selected = models ?? MultiAreaModelSelection.Phase2;
        var session = new MultiAreaContinuation(
            generated.Experiment,
            selected,
            stage4,
            stage5,
            selected.IsPhase2 ? generated.Options : null,
            economy,
            genesisRecipe: generated.Recipe,
            scotsPine: selected.IsPhase2 ? generated.ScotsPineGenesisFor(siteMoisture) : null);
        return (generated, session);
    }

    // ---------------------------------------------------------------- algorithm steps (spec §5.2 rules 1-9 & §5.3)

    /// <summary>Rule 1 (§4): N equal parallel edge-to-edge lanes split along X with integer floor boundaries.</summary>
    private static LaneBounds[] LaneRectangles(long widthMm, long lengthMm, int laneCount)
    {
        var lanes = new LaneBounds[laneCount];
        for (int i = 0; i < laneCount; i++)
        {
            long x0 = i * widthMm / laneCount;
            long x1 = (i + 1) * widthMm / laneCount;
            char letter = (char)('A' + i);
            string areaId = "MA-" + letter;
            string label = "Lane " + letter;
            var polygon = Array.AsReadOnly(new[]
            {
                new MillimetrePoint(x0, 0L),
                new MillimetrePoint(x1, 0L),
                new MillimetrePoint(x1, lengthMm),
                new MillimetrePoint(x0, lengthMm),
            });
            lanes[i] = new LaneBounds(i, areaId, label, x0, x1, lengthMm, polygon);
        }
        return lanes;
    }

    /// <summary>Rule 7: refuse any interior lane boundary falling on a 2.5 m light-cell or 0.25 m micro-cell centre line.</summary>
    internal static void RequireGridAlignment(long widthMm, int laneCount)
    {
        for (int i = 1; i < laneCount; i++)
        {
            long x = i * widthMm / laneCount;
            bool hitsLightCentre = x % ForestMakerBounds.LightCellSizeMm == ForestMakerBounds.LightCellHalfMm;
            bool hitsMicroCentre = x % ForestMakerBounds.MicroCellSizeMm == ForestMakerBounds.MicroCellHalfMm;
            if (hitsLightCentre || hitsMicroCentre)
            {
                throw new P1Refusal("ForestMakerLaneGridAlignment",
                    string.Create(CultureInfo.InvariantCulture,
                        $"interior lane boundary {i} at x={x} mm aligns with a light/micro cell centre line (widthMm={widthMm}, lanes={laneCount})"));
            }
        }
    }

    /// <summary>Rule 2: split TotalStems across lanes by largest remainder on lane width; ties go to lower lane index.</summary>
    internal static int[] ComputeLaneQuotas(int totalStems, long widthMm, int laneCount)
    {
        var floors = new int[laneCount];
        var remainders = new long[laneCount];
        int allocated = 0;

        for (int i = 0; i < laneCount; i++)
        {
            long x0 = i * widthMm / laneCount;
            long x1 = (i + 1) * widthMm / laneCount;
            long w = x1 - x0;
            long numerator = (long)totalStems * w;
            floors[i] = checked((int)(numerator / widthMm));
            remainders[i] = numerator % widthMm;
            allocated += floors[i];
        }

        int leftover = totalStems - allocated;
        if (leftover > 0)
        {
            int[] order = Enumerable.Range(0, laneCount)
                .OrderByDescending(i => remainders[i])
                .ThenBy(i => i)
                .ToArray();
            for (int k = 0; k < leftover; k++)
                floors[order[k]]++;
        }

        return floors;
    }

    internal static void RequireNonEmptyQuotas(int[] quotas)
    {
        for (int i = 0; i < quotas.Length; i++)
        {
            if (quotas[i] <= 0)
                throw new P1Refusal("ForestMakerLaneEmpty", $"lane MA-{(char)('A' + i)} has a zero stem quota");
        }
    }

    /// <summary>Rule 3: 4 age-60 edge broadleaves on west and east outer edges, alternating Oak/Beech.</summary>
    private static PlacedStem[][] EdgeBroadleaves(ForestMakerRecipe recipe, LaneBounds[] lanes)
    {
        var byLane = new PlacedStem[lanes.Length][];
        for (int i = 0; i < lanes.Length; i++) byLane[i] = Array.Empty<PlacedStem>();
        if (recipe.SpeciesMix != ForestMakerSpeciesMix.SitkaPlusEdgeBroadleaves) return byLane;

        const int perEdge = ForestMakerBounds.EdgeBroadleavesPerOuterEdge;
        long lengthMm = recipe.LengthMm;
        long westX = ForestMakerBounds.EdgeInsetMm;
        long eastX = recipe.WidthMm - ForestMakerBounds.EdgeInsetMm;

        var west = new PlacedStem[perEdge];
        var east = new PlacedStem[perEdge];
        int lastLane = lanes.Length - 1;

        for (int k = 0; k < perEdge; k++)
        {
            long yMm = lengthMm * (2L * k + 1L) / 8L;

            string westSpecies = k % 2 == 0 ? Phase2Species.OakId : Phase2Species.BeechId;
            var (westDbh, westHeight) = EdgeBroadleafStartingSize(recipe.Seed, 0, k);
            west[k] = new PlacedStem(westX, yMm, ForestMakerBounds.EdgeBroadleafAgeYears, westDbh, westHeight, westSpecies);

            string eastSpecies = k % 2 == 0 ? Phase2Species.BeechId : Phase2Species.OakId;
            var (eastDbh, eastHeight) = EdgeBroadleafStartingSize(recipe.Seed, lastLane, k);
            east[k] = new PlacedStem(eastX, yMm, ForestMakerBounds.EdgeBroadleafAgeYears, eastDbh, eastHeight, eastSpecies);
        }

        byLane[0] = west;
        byLane[lastLane] = east;
        return byLane;
    }

    /// <summary>Rules 4 &amp; 5: jittered lattice candidates inside lane margin and outside broadleaf exclusion, keeping smallest SplitMix keys.</summary>
    internal static IReadOnlyList<LatticeCandidate> SelectExact(int seed, LaneBounds lane, int quota, int needed,
        PlacedStem[] allEdgeBroadleaves)
    {
        if (needed == 0) return Array.Empty<LatticeCandidate>();

        int cols = Math.Max(1, ISqrt((long)quota * lane.WidthMm / lane.LengthMm));
        int baseRows = (quota + cols - 1) / cols;
        int maxRows = baseRows * 2;

        for (int rows = baseRows; rows <= maxRows; rows++)
        {
            List<LatticeCandidate> candidates = SitkaCandidates(seed, lane, cols, rows, allEdgeBroadleaves);
            if (candidates.Count >= needed)
            {
                return candidates
                    .OrderBy(c => c.SelectionKey)
                    .ThenBy(c => c.YMm)
                    .ThenBy(c => c.XMm)
                    .Take(needed)
                    .ToArray();
            }
        }

        throw new P1Refusal("ForestMakerInfeasibleStocking",
            $"{lane.AreaId} could not place {needed} stems inside lane margins and broadleaf exclusion up to {maxRows} rows");
    }

    private static List<LatticeCandidate> SitkaCandidates(int seed, LaneBounds lane, int cols, int rows,
        PlacedStem[] allEdgeBroadleaves)
    {
        var candidates = new List<LatticeCandidate>(cols * rows);
        long pX = lane.WidthMm / cols;
        long pY = lane.LengthMm / rows;
        if (pX <= 0 || pY <= 0) return candidates;

        long jX = pX / 5;
        long jY = pY / 5;
        long exclusionSq = ForestMakerBounds.ExclusionRadiusMm * ForestMakerBounds.ExclusionRadiusMm;
        var seen = new HashSet<(long, long)>();

        for (int row = 0; row < rows; row++)
        {
            long baseY = pY / 2 + row * pY;
            for (int col = 0; col < cols; col++)
            {
                int cell = row * cols + col;
                long baseX = lane.X0Mm + pX / 2 + col * pX;
                long x = baseX + Jitter(seed, lane.Index, cell, 1, jX);
                long y = baseY + Jitter(seed, lane.Index, cell, 2, jY);

                if (!MultiAreaGeometry.InsideWithMargin(lane.Polygon, new MillimetrePoint(x, y), ForestMakerBounds.EdgeMarginMm))
                    continue;
                if (!IsOutsideBroadleafExclusion(x, y, allEdgeBroadleaves, exclusionSq))
                    continue;
                if (!seen.Add((x, y)))
                    continue;

                ulong key = Next(seed, lane.Index, cell, 90);
                candidates.Add(new LatticeCandidate(x, y, cell, key));
            }
        }

        return candidates;
    }

    private static bool IsOutsideBroadleafExclusion(long xMm, long yMm, PlacedStem[] broadleaves, long exclusionSq)
    {
        foreach (var b in broadleaves)
        {
            long dx = xMm - b.XMm;
            long dy = yMm - b.YMm;
            if (dx * dx + dy * dy < exclusionSq) return false;
        }
        return true;
    }

    // [Calibration] Scots pine share of cohort stems on the dry ridge (x below the stand midpoint) and on the moist slope.
    internal const int ScotsPineDryRidgePercent = 75;
    internal const int ScotsPineMoistSlopePercent = 25;

    private static string PickCohortSpecies(ForestMakerRecipe recipe, int laneIndex, int cellIndex, long xMm)
    {
        if (recipe.SpeciesMix == ForestMakerSpeciesMix.ScotsPineSitkaTwoZone)
        {
            // Same midpoint rule as the Part 1 Dry Ridge & Moist Slope map: x below W/2 is dry, the midpoint and above is moist.
            int pinePercent = 2 * xMm < recipe.WidthMm ? ScotsPineDryRidgePercent : ScotsPineMoistSlopePercent;
            int draw = (int)(Next(recipe.Seed, laneIndex, cellIndex, 30) % 100UL);
            return draw < pinePercent ? ScotsPineLive.SpeciesId : Phase2Species.SitkaId;
        }
        if (recipe.SpeciesMix != ForestMakerSpeciesMix.CustomSplit)
            return Phase2Species.SitkaId;

        int percentile = (int)(Next(recipe.Seed, laneIndex, cellIndex, 30) % 100UL);
        if (percentile < recipe.OakPercent) return Phase2Species.OakId;
        if (percentile < recipe.OakPercent + recipe.BeechPercent) return Phase2Species.BeechId;
        return Phase2Species.SitkaId;
    }

    /// <summary>Starting DBH and height for cohort stems (spec §5.3, age 20 Enlarged80 transfer &amp; Q1 piecewise-linear table).</summary>
    internal static (long DbhMm, long HeightMm) CohortStartingSize(int seed, int laneIndex, int cellIndex, int ageYears)
    {
        long meanDbhMm = MeanDbhForAgeMm(ageYears);
        long spreadPer1000 = meanDbhMm / 4L;
        long minDbhMm = meanDbhMm / 2L;
        long maxDbhMm = 3L * meanDbhMm / 2L;

        long sum = IrwinHall4Sum(seed, laneIndex, cellIndex, 10);
        long dbhMm = Math.Clamp(meanDbhMm + (sum - 2_000L) * spreadPer1000 / 1_000L, minDbhMm, maxDbhMm);
        long heightMm = Math.Clamp(
            Math.Max(CohortMinHeightMm, CohortHeightInterceptMm + CohortHeightPerDbhMm * dbhMm),
            DerivedManifest.MinHeightMm,
            DerivedManifest.MaxHeightMm);
        return (dbhMm, heightMm);
    }

    /// <summary>Starting DBH and height for age-60 edge broadleaves (spec §5.3 Q2 accepted default: MA-1 Area B older cohort).</summary>
    internal static (long DbhMm, long HeightMm) EdgeBroadleafStartingSize(int seed, int laneIndex, int edgeIndex)
    {
        long sum = IrwinHall4Sum(seed, laneIndex, edgeIndex, 50);
        long dbhMm = Math.Clamp(
            EdgeBroadleafMeanDbhMm + (sum - 2_000L) * EdgeBroadleafSpreadPer1000 / 1_000L,
            EdgeBroadleafMinDbhMm,
            EdgeBroadleafMaxDbhMm);
        long heightMm = Math.Clamp(
            EdgeBroadleafHeightAtMeanMm + EdgeBroadleafHeightSlopePerDbhMm * (dbhMm - EdgeBroadleafMeanDbhMm),
            DerivedManifest.MinHeightMm,
            DerivedManifest.MaxHeightMm);
        return (dbhMm, heightMm);
    }

    /// <summary>Accepted default Q1 piecewise-linear mean DBH (mm) by cohort age (10..60).</summary>
    internal static long MeanDbhForAgeMm(int ageYears)
    {
        for (int i = 0; i < CohortMeanDbhAnchors.Length - 1; i++)
        {
            var (a0, d0) = CohortMeanDbhAnchors[i];
            var (a1, d1) = CohortMeanDbhAnchors[i + 1];
            if (ageYears <= a1)
                return d0 + (d1 - d0) * (ageYears - a0) / (a1 - a0);
        }
        return CohortMeanDbhAnchors[^1].MeanDbhMm;
    }

    /// <summary>Rule 6: sort stems by (yMm, xMm) ascending and assign 4-digit ordinal IDs (<c>MA-A-0001</c>).</summary>
    private static (IReadOnlyList<MultiAreaTreeDefinition> Trees, int Sitka, int Oak, int Beech, int Pine) AssignIds(
        string areaId, List<PlacedStem> stems, Dictionary<string, string> genesisSpecies, List<string> scotsPineIds)
    {
        stems.Sort((a, b) =>
        {
            int cmpY = a.YMm.CompareTo(b.YMm);
            return cmpY != 0 ? cmpY : a.XMm.CompareTo(b.XMm);
        });

        var trees = new MultiAreaTreeDefinition[stems.Count];
        int sitka = 0, oak = 0, beech = 0, pine = 0;

        for (int i = 0; i < stems.Count; i++)
        {
            PlacedStem stem = stems[i];
            string treeId = areaId + "-" + (i + 1).ToString("D4", CultureInfo.InvariantCulture);
            trees[i] = new MultiAreaTreeDefinition(treeId, stem.XMm, stem.YMm, stem.AgeYears, stem.DbhMm, stem.HeightMm);

            if (string.Equals(stem.SpeciesId, Phase2Species.OakId, StringComparison.Ordinal))
            {
                oak++;
                genesisSpecies[treeId] = Phase2Species.OakId;
            }
            else if (string.Equals(stem.SpeciesId, Phase2Species.BeechId, StringComparison.Ordinal))
            {
                beech++;
                genesisSpecies[treeId] = Phase2Species.BeechId;
            }
            else if (ScotsPineLive.IsPine(stem.SpeciesId))
            {
                // Genesis Scots pine never enter Phase2Options.GenesisSpecies (frozen Oak/Beech contract); they are listed apart.
                pine++;
                scotsPineIds.Add(treeId);
            }
            else
            {
                sitka++;
            }
        }

        return (Array.AsReadOnly(trees), sitka, oak, beech, pine);
    }

    /// <summary>Rule 8: unique per-lane simulation seeds never equal to recipe.Seed.</summary>
    internal static int[] LaneSeeds(int seed, int laneCount)
    {
        var seeds = new int[laneCount];
        for (int i = 0; i < laneCount; i++)
            seeds[i] = checked(seed * 10 + i + 1);
        return seeds;
    }

    // The Scots pine count is appended only on the two-zone mix, so every existing stocking note and content hash is unchanged.
    private static string BuildStockingNote(ForestMakerRecipe recipe, LaneBounds lane, int sitka, int oak, int beech, int pine) =>
        string.Create(CultureInfo.InvariantCulture,
            $"forest-maker v{recipe.GeneratorVersion} lane {lane.AreaId} ({lane.WidthMm}x{lane.LengthMm} mm): " +
            $"target {recipe.StemsPerHa}/ha, age {recipe.StartAgeYears}, {sitka} sitka, {oak} oak, {beech} beech") +
        (recipe.SpeciesMix == ForestMakerSpeciesMix.ScotsPineSitkaTwoZone
            ? string.Create(CultureInfo.InvariantCulture, $", {pine} scots pine")
            : "");

    // ---------------------------------------------------------------- pure integer math and SplitMix64

    internal static int ISqrt(long value)
    {
        if (value <= 0) return 0;
        long r = (long)Math.Sqrt(value);
        while ((r + 1) * (r + 1) <= value) r++;
        while (r * r > value) r--;
        return checked((int)r);
    }

    private static long IrwinHall4Sum(int seed, int laneIndex, int cellIndex, int baseSalt)
    {
        long sum = 0;
        for (int k = 0; k < 4; k++)
            sum += (long)(Next(seed, laneIndex, cellIndex, baseSalt + k) % 1_001UL);
        return sum;
    }

    private static long Jitter(int seed, int laneIndex, int cellIndex, int salt, long maxAbsMm) =>
        maxAbsMm <= 0 ? 0L : (long)(Next(seed, laneIndex, cellIndex, salt) % (ulong)(2L * maxAbsMm + 1L)) - maxAbsMm;

    private static ulong Next(int seed, int laneIndex, int cellIndex, int salt)
    {
        ulong z = unchecked((ulong)(long)seed * 0x9E3779B97F4A7C15UL);
        z = SplitMix(z ^ (ulong)(uint)laneIndex);
        z = SplitMix(z ^ ((ulong)(uint)cellIndex << 16));
        return SplitMix(z ^ ((ulong)(uint)salt << 40));
    }

    private static ulong SplitMix(ulong z)
    {
        unchecked
        {
            z += 0x9E3779B97F4A7C15UL;
            z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9UL;
            z = (z ^ (z >> 27)) * 0x94D049BB133111EBUL;
            return z ^ (z >> 31);
        }
    }
}
