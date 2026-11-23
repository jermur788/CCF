namespace SitkaP1.Core;

/// <summary>One area's input to a Stage 5 step: identity, polygon, simulation seed and its roster after the adult step.</summary>
internal sealed record Stage5AreaInput(string AreaId, int SimulationSeed, IReadOnlyList<MultiAreaVertex> Polygon, float AreaHa,
    IReadOnlyList<AnnualTree> Trees);

/// <summary>The outcome of one Stage 5 year: the next state, the new adults per area and the year's account.</summary>
internal sealed record Stage5AnnualResult(MultiAreaStage5State Next,
    IReadOnlyDictionary<string, IReadOnlyList<AnnualTree>> PromotedTrees, Stage5YearAccount Account);

/// <summary>
/// The Stage 5 annual steps 5a–12 (spec §3), run after the unchanged Stage 4 adult step of every area. It is a pure function
/// of the start-of-year Stage 5 state, every area's post-adult-step roster and the light field of that woodland: the light,
/// the seed sources and the promotion competitors are read from the whole connected woodland at once, and areas are matched
/// by ID, so the order the areas are given in cannot change any result. It never mutates its inputs.
/// </summary>
internal static class Stage5Annual
{
    /// <summary>
    /// Year-0 state: every in-polygon cell with bramble = bracken = 0.25 · T(light) and no regeneration yet, and every aligned
    /// 5 m block that holds an in-polygon cell, with no promotion credit yet (spec §4 Option A2).
    /// </summary>
    public static MultiAreaStage5State Genesis(Stage5Options options, int mastSeed, IReadOnlyList<Stage5AreaInput> areas)
    {
        options.RequireValid();
        var ordered = areas.OrderBy(area => area.AreaId, StringComparer.Ordinal).ToArray();
        var light = ModelLight(ordered, options.GridCellM);
        var cells = ordered.Select(area => new Stage5AreaCells(area.AreaId, Array.AsReadOnly(light[area.AreaId].Select(cell =>
        {
            float cover = Stage5Undergrowth.Initial(cell.LightIndex);
            return new Stage5Cell(cell.Column, cell.Row, cell.CentreXM, cell.CentreZM, cell.ClippedAreaM2, cell.LightIndex,
                cover, cover, Array.Empty<Stage5Band>());
        }).ToArray()))).ToArray();
        // A block straddling two areas counts the in-polygon cells of both (cells of different areas never coincide).
        var blocks = cells.SelectMany(area => area.Cells)
            .GroupBy(cell => Stage5Promotion.BlockOf(cell.Column, cell.Row, options.GridCellM))
            .OrderBy(group => group.Key.X).ThenBy(group => group.Key.Z)
            .Select(group => new Stage5BlockCredit(group.Key.X, group.Key.Z, group.Count(), 0))
            .ToArray();
        return new MultiAreaStage5State(options, mastSeed, Array.AsReadOnly(cells), Array.Empty<MultiAreaClearUndergrowthOrder>(),
            Array.Empty<MultiAreaClearUndergrowthOrder>(), Array.Empty<MultiAreaClearUndergrowthRecord>(), Array.Empty<Stage5YearAccount>(),
            Array.AsReadOnly(blocks));
    }

    /// <summary>
    /// One Stage 5 year ending in ecological year <paramref name="yearAfter"/>, reading the model light of the post-adult-step
    /// woodland (spec §3 step 5). This is the only overload a session uses.
    /// </summary>
    public static Stage5AnnualResult Step(MultiAreaStage5State start, IReadOnlyList<Stage5AreaInput> areas, int yearAfter) =>
        Step(start, areas, yearAfter, ModelLight(InStateOrder(start, areas), start.Options.GridCellM));

    /// <summary>
    /// The same year with the light field given: the light index of every in-polygon cell of every area, keyed by area ID,
    /// in each area's lattice order. Every session calls the overload above, which passes the model light, and nothing in
    /// <see cref="Stage5Options"/> or the session can select another field. This overload lets the S5-07A qualification
    /// control give the 2.5 m and 5.0 m grids one identical light field.
    /// </summary>
    public static Stage5AnnualResult Step(MultiAreaStage5State start, IReadOnlyList<Stage5AreaInput> areas, int yearAfter,
        IReadOnlyDictionary<string, IReadOnlyList<CanopyLightCell>> lightByArea)
    {
        var options = start.Options;
        var scale = Stage5CellScale.For(options.GridCellM);
        var inputs = InStateOrder(start, areas);
        var light = inputs.Select(area => lightByArea.TryGetValue(area.AreaId, out var cells) ? cells
            : throw new P1Refusal("Stage5LightMissing", "the Stage 5 step needs the light of " + area.AreaId)).ToArray();

        // Steps 7–8: one shared mast roll, then the seed sources of the post-adult-step woodland.
        var woodland = CrownModel.Woodland(inputs.SelectMany(area => area.Trees));
        var mast = Stage5Mast.ForYear(start.MastSeed, yearAfter);
        var sources = Stage5SeedRain.Sources(woodland, mast.Multiplier);

        var work = new List<Stage5BandWork>[inputs.Length][];
        var covers = new (float Bramble, float Bracken)[inputs.Length][];
        var tallies = new AreaTally[inputs.Length];
        for (int i = 0; i < inputs.Length; i++)
        {
            var cells = start.Areas[i].Cells;
            RequireSameLattice(cells, light[i], start.Areas[i].AreaId);
            work[i] = new List<Stage5BandWork>[cells.Count];
            covers[i] = new (float, float)[cells.Count];
            var tally = tallies[i] = new AreaTally(cells.Count);
            for (int c = 0; c < cells.Count; c++)
            {
                var cell = cells[c];
                float cellLight = light[i][c].LightIndex;
                // 5a. Bramble and bracken move toward this year's light target.
                float target = Stage5Undergrowth.Target(cellLight);
                float bramble = Stage5Undergrowth.Step(cell.Bramble, target);
                float bracken = Stage5Undergrowth.Step(cell.Bracken, target);
                covers[i][c] = (bramble, bracken);
                // 6. Existing bands grow and survive before any new establishment.
                var bands = cell.Bands.Select(Stage5BandWork.From).ToList();
                GrowAndSurvive(bands, cellLight, bramble, bracken, options.BackgroundBrowsePressure, yearAfter, tally);
                Stage5Bands.Normalize(bands, scale, tally.Bands);
                // 8–9. Seed rain at the cell centre, then establishment into this year's band.
                float seedRain = Stage5SeedRain.SeedRainAt(cell.CentreXM, cell.CentreZM, sources);
                if (seedRain > 0f) tally.SeedArrival += seedRain;
                float requested = Stage5Bands.RequestedEstablishment(seedRain, cellLight, scale);
                if (requested > 0f)
                    tally.CellAccepted[c] = Stage5Bands.AdmitRecruitment(bands, yearAfter, Stage5Constants.RegenInitialHeightM, requested,
                        scale, tally.Bands);
                work[i][c] = bands;
            }
        }

        // 10. Promotion under the block rules of the whole woodland (spec §4, owner Option A).
        var promoted = Promote(start, inputs, light, work, woodland, scale, yearAfter, tallies, out var nextBlocks);

        // 11–12. Neutral suitability (a constant 1.0, no state) and the read-only accounts.
        var nextAreas = new Stage5AreaCells[inputs.Length];
        var accounts = new Stage5AreaAccount[inputs.Length];
        for (int i = 0; i < inputs.Length; i++)
        {
            var cells = start.Areas[i].Cells;
            var next = new Stage5Cell[cells.Count];
            for (int c = 0; c < cells.Count; c++)
                next[c] = cells[c] with
                {
                    Light = light[i][c].LightIndex,
                    Bramble = covers[i][c].Bramble,
                    Bracken = covers[i][c].Bracken,
                    Bands = Array.AsReadOnly(work[i][c].Select(band => band.Freeze()).ToArray()),
                };
            nextAreas[i] = new Stage5AreaCells(start.Areas[i].AreaId, Array.AsReadOnly(next));
            accounts[i] = tallies[i].ToAccount(start.Areas[i].AreaId, next, work[i], scale);
        }
        var account = new Stage5YearAccount(yearAfter - 1, yearAfter, mast.Roll, mast.Label, mast.Multiplier, Array.AsReadOnly(accounts));
        var nextState = start with
        {
            Areas = Array.AsReadOnly(nextAreas),
            Accounts = Array.AsReadOnly(start.Accounts.Append(account).ToArray()),
            Blocks = Array.AsReadOnly(nextBlocks),
        };
        return new Stage5AnnualResult(nextState, promoted, account);
    }

    // Step 6 for every band of one cell, in band order (Unity GrowExistingRegeneration, regeneration model 1 plus the
    // bramble/bracken penalty). Browse probability and p_veg both use the band's pre-growth height (owner Q6, spec §3.1).
    private static void GrowAndSurvive(List<Stage5BandWork> bands, float light, float bramble, float bracken, float pressure,
        int yearAfter, AreaTally tally)
    {
        foreach (var band in bands)
        {
            if (band.Density <= 0f) continue;
            float preGrowthHeight = band.Height;
            float browsed = pressure > 0f ? Stage5Juvenile.BrowseProbability(pressure, preGrowthHeight) : 0f;
            band.LastBrowsedFraction = browsed;
            band.LastBrowseAssessmentYear = yearAfter;
            tally.StartingAbundance += band.Density;
            float vegetationLoss = Stage5Undergrowth.VegetationLossProbability(bramble, bracken, preGrowthHeight);
            band.Height = Stage5Juvenile.GrowHeight(preGrowthHeight, light, browsed);
            float before = band.Density;
            // Accounting only: light loss first, then the extra browse loss, then the bramble/bracken loss.
            float lightOnly = (float)(before * Stage5Juvenile.LightSurvival(light));
            float lightAndBrowse = (float)(before * Stage5Juvenile.LightBrowseSurvival(light, browsed));
            band.Density = (float)(before * Stage5Juvenile.Survival(light, browsed, vegetationLoss));
            tally.LightLoss += before - lightOnly;
            tally.BrowseLoss += lightOnly - lightAndBrowse;
            tally.VegetationLoss += lightAndBrowse - band.Density;
        }
    }

    // Step 10 (spec §4, owner Option A) for every aligned 5 m block of the whole woodland:
    //   A2 (1) the block's credit gains its in-polygon cell count;
    //      (2) if one of its cells qualifies and the credit holds a full block, the block winner becomes ONE adult and the
    //          credit pays one full block; otherwise nothing in the block promotes this year;
    //   A1     the block's other represented bands of the winner's cohort (same EstablishYear) leave with it into that tree;
    //   A2 (3) unspent credit is capped at one promotion.
    // A full block (every 5.0 m cell is one) always holds a full block of credit after (1), so its budget never binds, and a
    // 5.0 m block has no other cell to export from: the 5.0 m behaviour is the first-approval rule. Blocks are disjoint, so the
    // order they are visited in cannot change any result.
    private static IReadOnlyDictionary<string, IReadOnlyList<AnnualTree>> Promote(MultiAreaStage5State start, Stage5AreaInput[] inputs,
        IReadOnlyList<CanopyLightCell>[] light, List<Stage5BandWork>[][] work, AnnualTree[] woodland, Stage5CellScale scale,
        int yearAfter, AreaTally[] tallies, out Stage5BlockCredit[] nextBlocks)
    {
        double cellM = start.Options.GridCellM;
        int fullBlock = Stage5Promotion.CellsPerBlock(cellM);
        var blockCells = new Dictionary<(int X, int Z), List<(int Area, int Cell)>>();
        var candidates = new List<Stage5PromotionCandidate>();
        for (int i = 0; i < inputs.Length; i++)
        {
            var cells = start.Areas[i].Cells;
            for (int c = 0; c < cells.Count; c++)
            {
                var block = Stage5Promotion.BlockOf(cells[c].Column, cells[c].Row, cellM);
                if (!blockCells.TryGetValue(block, out var members)) blockCells[block] = members = new List<(int, int)>();
                members.Add((i, c));
                var band = Stage5Promotion.OldestQualifyingBand(work[i][c], light[i][c].LightIndex, scale);
                if (band is null || band.Density <= 0f) continue;
                candidates.Add(new Stage5PromotionCandidate(i, inputs[i].AreaId, c, cells[c].Column, cells[c].Row, block.X, block.Z,
                    band.EstablishYear, band.Height));
                tallies[i].QualifyingCells++;
            }
        }

        // A2 (1): this year's budget accrues, in cell-years.
        var credit = new Dictionary<(int X, int Z), int>(start.Blocks.Count);
        foreach (var block in start.Blocks) credit.Add((block.BlockX, block.BlockZ), checked(block.Credit + block.InPolygonCells));

        var existingIds = inputs.SelectMany(area => area.Trees).Select(tree => tree.TreeId).ToHashSet(StringComparer.Ordinal);
        var promoted = inputs.ToDictionary(area => area.AreaId, _ => new List<AnnualTree>(), StringComparer.Ordinal);
        var bandsGone = new HashSet<(int Area, int Cell, int EstablishYear)>(); // promoted or exported this year
        foreach (var winner in Stage5Promotion.Winners(candidates))
        {
            var key = (winner.BlockX, winner.BlockZ);
            if (!credit.TryGetValue(key, out int available))
                throw new InvalidOperationException("Stage 5 promotion block without a credit record.");
            // A2 (2): without a full block of credit the block's bands wait for a later year.
            if (available < fullBlock)
            {
                tallies[winner.AreaOrder].BudgetWithheldBlocks++;
                continue;
            }
            credit[key] = available - fullBlock;

            var input = inputs[winner.AreaOrder];
            var cell = start.Areas[winner.AreaOrder].Cells[winner.CellIndex];
            var bands = work[winner.AreaOrder][winner.CellIndex];
            var band = Stage5Promotion.OldestQualifyingBand(bands, light[winner.AreaOrder][winner.CellIndex].LightIndex, scale)!;
            string treeId = Stage5Promotion.TreeIdFor(yearAfter, input.AreaId, cell.Column, cell.Row);
            if (!existingIds.Add(treeId))
                throw new P1Refusal("Stage5TreeIdNotUnique", "a promoted tree ID already exists: " + treeId);
            var (x, z, fallback) = Stage5Promotion.Place(input.Polygon, cell.CentreXM, cell.CentreZM, cellM, input.AreaId,
                cell.Column, cell.Row, yearAfter, input.SimulationSeed);
            var (tree, ci) = Stage5Promotion.NewAdult(treeId, x, z, yearAfter, band.EstablishYear, band.Height, woodland);
            // The winning band's abundance is exported to exactly one tree and the band is removed (Unity, not a stem count).
            bands.Remove(band);
            bandsGone.Add((winner.AreaOrder, winner.CellIndex, band.EstablishYear));
            promoted[input.AreaId].Add(tree);
            tallies[winner.AreaOrder].PromotionExported += band.Density;

            // A1: the same cohort in the block's other cells leaves with it. Each such band is in exactly one cell and is
            // removed here once, so its abundance is exported once, into this one tree, and booked to its own area.
            int cohortBands = 0;
            double cohortDensity = 0.0;
            foreach (var (siblingArea, siblingCell) in blockCells[key])
            {
                if (siblingArea == winner.AreaOrder && siblingCell == winner.CellIndex) continue;
                var sibling = Stage5Promotion.CohortBand(work[siblingArea][siblingCell], band.EstablishYear, scale);
                if (sibling is null) continue;
                work[siblingArea][siblingCell].Remove(sibling);
                bandsGone.Add((siblingArea, siblingCell, sibling.EstablishYear));
                cohortBands++;
                cohortDensity += sibling.Density;
                tallies[siblingArea].PromotionExported += sibling.Density;
                tallies[siblingArea].CohortBandsExported++;
            }

            tallies[winner.AreaOrder].Promotions.Add(new Stage5PromotionRecord(treeId, input.AreaId, cell.Column, cell.Row,
                winner.BlockX, winner.BlockZ, band.EstablishYear, tree.AgeYears, tree.HeightM, tree.DbhCm, tree.CrownRadiusM, ci,
                tree.XM, tree.ZM, fallback, band.Density, cohortBands, cohortDensity));
        }
        // A candidate whose qualifying band is still in its cell waits for a later year.
        foreach (var candidate in candidates)
            if (!bandsGone.Contains((candidate.AreaOrder, candidate.CellIndex, candidate.EstablishYear)))
                tallies[candidate.AreaOrder].BlockCapDeferredCells++;

        // A2 (3): unspent credit never exceeds one promotion (the unit of the hard cap).
        nextBlocks = start.Blocks.Select(block => block with { Credit = Math.Min(credit[(block.BlockX, block.BlockZ)], fullBlock) })
            .ToArray();
        return promoted.ToDictionary(pair => pair.Key, pair => (IReadOnlyList<AnnualTree>)pair.Value.AsReadOnly(), StringComparer.Ordinal);
    }

    // Areas are matched by ID and processed in the state's own (ordinal) order, whatever order the caller used.
    private static Stage5AreaInput[] InStateOrder(MultiAreaStage5State start, IReadOnlyList<Stage5AreaInput> areas)
    {
        var byId = areas.ToDictionary(area => area.AreaId, StringComparer.Ordinal);
        if (byId.Count != start.Areas.Count || start.Areas.Any(area => !byId.ContainsKey(area.AreaId)))
            throw new P1Refusal("Stage5AreasMismatch", "the Stage 5 step needs exactly the areas of its state");
        return start.Areas.Select(area => byId[area.AreaId]).ToArray();
    }

    // The model light of every cell of every area at its own cell centre (the unchanged canopy/light evaluator, Layer C),
    // from the given rosters as one connected woodland, keyed by area ID in each area's lattice order.
    private static IReadOnlyDictionary<string, IReadOnlyList<CanopyLightCell>> ModelLight(IReadOnlyList<Stage5AreaInput> areas,
        double gridCellM)
    {
        var canopyAreas = areas.Select(area => new CanopyArea(area.AreaId,
            Array.AsReadOnly(area.Polygon.Select(v => new CanopyVertex(v.XM, v.ZM)).ToArray()), area.AreaHa, area.Trees)).ToArray();
        var fields = CanopyLightEvaluator.LightFields(canopyAreas, gridCellM);
        return areas.Select((area, i) => (area.AreaId, fields[i].Cells))
            .ToDictionary(pair => pair.AreaId, pair => pair.Cells, StringComparer.Ordinal);
    }

    private static void RequireSameLattice(IReadOnlyList<Stage5Cell> cells, IReadOnlyList<CanopyLightCell> light, string areaId)
    {
        if (cells.Count != light.Count)
            throw new P1Refusal("Stage5LatticeChanged", "the light lattice of " + areaId + " no longer matches its Stage 5 cells");
        for (int c = 0; c < cells.Count; c++)
            if (cells[c].Column != light[c].Column || cells[c].Row != light[c].Row)
                throw new P1Refusal("Stage5LatticeChanged", "the light lattice of " + areaId + " no longer matches its Stage 5 cells");
    }

    /// <summary>One area's running totals while a year is built (diagnostics only).</summary>
    private sealed class AreaTally
    {
        public readonly Stage5BandCounters Bands = new();
        public readonly List<Stage5PromotionRecord> Promotions = new();
        public readonly float[] CellAccepted;
        public double SeedArrival, StartingAbundance, VegetationLoss, LightLoss, BrowseLoss, PromotionExported;
        public int QualifyingCells, BlockCapDeferredCells, CohortBandsExported, BudgetWithheldBlocks;

        public AreaTally(int cells) => CellAccepted = new float[cells];

        public Stage5AreaAccount ToAccount(string areaId, Stage5Cell[] cells, List<Stage5BandWork>[] work, Stage5CellScale scale)
        {
            int n = cells.Length;
            double ending = cells.Sum(cell => cell.Abundance);
            double occupancy = work.Sum(bands => (double)Stage5Bands.SharedOccupancy(bands, scale));
            return new Stage5AreaAccount(areaId, n, SeedArrival, StartingAbundance, VegetationLoss, LightLoss, BrowseLoss,
                Bands.EstablishmentRequested, Bands.EstablishmentAccepted, Bands.CapacityRejected, Bands.BandsCreated,
                Bands.BandMerges, Bands.ThresholdCrossings, QualifyingCells, BlockCapDeferredCells, PromotionExported, ending,
                n == 0 ? 0.0 : occupancy / n, n == 0 ? 0.0 : cells.Average(cell => (double)cell.Bramble),
                n == 0 ? 0.0 : cells.Average(cell => (double)cell.Bracken), n == 0 ? 0.0 : cells.Average(cell => (double)cell.Light),
                Promotions.AsReadOnly(), CohortBandsExported, BudgetWithheldBlocks, Array.AsReadOnly(CellAccepted));
        }
    }
}
