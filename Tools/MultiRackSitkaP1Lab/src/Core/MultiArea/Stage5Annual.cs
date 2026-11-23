namespace SitkaP1.Core;

/// <summary>One area's input to a Stage 5 step: identity, polygon, simulation seed and its roster after the adult step.</summary>
internal sealed record Stage5AreaInput(string AreaId, int SimulationSeed, IReadOnlyList<MultiAreaVertex> Polygon, float AreaHa,
    IReadOnlyList<AnnualTree> Trees);

/// <summary>The outcome of one Stage 5 year: the next state, the new adults per area and the year's account.</summary>
internal sealed record Stage5AnnualResult(MultiAreaStage5State Next,
    IReadOnlyDictionary<string, IReadOnlyList<AnnualTree>> PromotedTrees, Stage5YearAccount Account);

/// <summary>
/// The Stage 5 annual steps 5a–12 (spec §3), run after the unchanged Stage 4 adult step of every area. It is a pure function
/// of the start-of-year Stage 5 state and every area's post-adult-step roster: the light field, the seed sources and the
/// promotion competitors are read from the whole connected woodland at once, and areas are matched by ID, so the order the
/// areas are given in cannot change any result. It never mutates its inputs.
/// </summary>
internal static class Stage5Annual
{
    /// <summary>Year-0 state: every in-polygon cell with bramble = bracken = 0.25 · T(light) and no regeneration yet.</summary>
    public static MultiAreaStage5State Genesis(Stage5Options options, int mastSeed, IReadOnlyList<Stage5AreaInput> areas)
    {
        options.RequireValid();
        var ordered = areas.OrderBy(area => area.AreaId, StringComparer.Ordinal).ToArray();
        var woodland = CrownModel.Woodland(ordered.SelectMany(area => area.Trees));
        var light = RefreshLight(ordered, options, woodland);
        var cells = ordered.Select((area, i) => new Stage5AreaCells(area.AreaId, Array.AsReadOnly(light[i].Select(cell =>
        {
            float cover = Stage5Undergrowth.Initial(cell.LightIndex);
            return new Stage5Cell(cell.Column, cell.Row, cell.CentreXM, cell.CentreZM, cell.ClippedAreaM2, cell.LightIndex,
                cover, cover, Array.Empty<Stage5Band>());
        }).ToArray()))).ToArray();
        return new MultiAreaStage5State(options, mastSeed, Array.AsReadOnly(cells), Array.Empty<MultiAreaClearUndergrowthOrder>(),
            Array.Empty<MultiAreaClearUndergrowthOrder>(), Array.Empty<MultiAreaClearUndergrowthRecord>(), Array.Empty<Stage5YearAccount>());
    }

    /// <summary>One Stage 5 year ending in ecological year <paramref name="yearAfter"/>.</summary>
    public static Stage5AnnualResult Step(MultiAreaStage5State start, IReadOnlyList<Stage5AreaInput> areas, int yearAfter)
    {
        var options = start.Options;
        var scale = Stage5CellScale.For(options.GridCellM);
        // Areas are matched by ID and processed in the state's own (ordinal) order, whatever order the caller used.
        var byId = areas.ToDictionary(area => area.AreaId, StringComparer.Ordinal);
        if (byId.Count != start.Areas.Count || start.Areas.Any(area => !byId.ContainsKey(area.AreaId)))
            throw new P1Refusal("Stage5AreasMismatch", "the Stage 5 step needs exactly the areas of its state");
        var inputs = start.Areas.Select(area => byId[area.AreaId]).ToArray();

        // Step 5: the refreshed light of the post-adult-step woodland. Steps 7–8: one shared mast roll, then seed sources.
        var woodland = CrownModel.Woodland(inputs.SelectMany(area => area.Trees));
        var light = RefreshLight(inputs, options, woodland);
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
            var tally = tallies[i] = new AreaTally();
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
                    Stage5Bands.AdmitRecruitment(bands, yearAfter, Stage5Constants.RegenInitialHeightM, requested, scale, tally.Bands);
                work[i][c] = bands;
            }
        }

        // 10. Promotion: at most one per aligned 5 m block of the whole woodland.
        var promoted = Promote(start, inputs, light, work, woodland, scale, yearAfter, tallies);

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

    private static IReadOnlyDictionary<string, IReadOnlyList<AnnualTree>> Promote(MultiAreaStage5State start, Stage5AreaInput[] inputs,
        IReadOnlyList<CanopyLightCell>[] light, List<Stage5BandWork>[][] work, AnnualTree[] woodland, Stage5CellScale scale,
        int yearAfter, AreaTally[] tallies)
    {
        double cellM = start.Options.GridCellM;
        var candidates = new List<Stage5PromotionCandidate>();
        for (int i = 0; i < inputs.Length; i++)
        {
            var cells = start.Areas[i].Cells;
            for (int c = 0; c < cells.Count; c++)
            {
                var band = Stage5Promotion.OldestQualifyingBand(work[i][c], light[i][c].LightIndex, scale);
                if (band is null || band.Density <= 0f) continue;
                var (blockX, blockZ) = Stage5Promotion.BlockOf(cells[c].Column, cells[c].Row, cellM);
                candidates.Add(new Stage5PromotionCandidate(i, inputs[i].AreaId, c, cells[c].Column, cells[c].Row, blockX, blockZ,
                    band.EstablishYear, band.Height));
                tallies[i].QualifyingCells++;
            }
        }
        var winners = Stage5Promotion.Winners(candidates);
        foreach (var candidate in candidates) tallies[candidate.AreaOrder].BlockCapDeferredCells++;
        foreach (var winner in winners) tallies[winner.AreaOrder].BlockCapDeferredCells--;

        var existingIds = inputs.SelectMany(area => area.Trees).Select(tree => tree.TreeId).ToHashSet(StringComparer.Ordinal);
        var promoted = inputs.ToDictionary(area => area.AreaId, _ => new List<AnnualTree>(), StringComparer.Ordinal);
        foreach (var winner in winners)
        {
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
            // The band's abundance is exported to exactly one tree and the band is removed (Unity, not a stem count).
            bands.Remove(band);
            promoted[input.AreaId].Add(tree);
            tallies[winner.AreaOrder].PromotionExported += band.Density;
            tallies[winner.AreaOrder].Promotions.Add(new Stage5PromotionRecord(treeId, input.AreaId, cell.Column, cell.Row,
                winner.BlockX, winner.BlockZ, band.EstablishYear, tree.AgeYears, tree.HeightM, tree.DbhCm, tree.CrownRadiusM, ci,
                tree.XM, tree.ZM, fallback, band.Density));
        }
        return promoted.ToDictionary(pair => pair.Key, pair => (IReadOnlyList<AnnualTree>)pair.Value.AsReadOnly(), StringComparer.Ordinal);
    }

    // The light of every cell of every area, in each area's lattice order. CellCentre is the model; the S5-07 control reads
    // every cell's light at the centre of its aligned 5 m block instead (identical to CellCentre on the 5.0 m grid).
    private static IReadOnlyList<CanopyLightCell>[] RefreshLight(IReadOnlyList<Stage5AreaInput> areas, Stage5Options options,
        AnnualTree[] woodland)
    {
        var canopyAreas = areas.Select(area => new CanopyArea(area.AreaId,
            Array.AsReadOnly(area.Polygon.Select(v => new CanopyVertex(v.XM, v.ZM)).ToArray()), area.AreaHa, area.Trees)).ToArray();
        var fields = CanopyLightEvaluator.LightFields(canopyAreas, options.GridCellM);
        if (options.LightSampling == Stage5LightSampling.CellCentre)
            return fields.Select(field => field.Cells).ToArray();
        return fields.Select(field => (IReadOnlyList<CanopyLightCell>)Array.AsReadOnly(field.Cells.Select(cell =>
        {
            var (blockX, blockZ) = Stage5Promotion.BlockOf(cell.Column, cell.Row, options.GridCellM);
            float x = (float)((blockX + 0.5) * Stage5Constants.PromotionBlockM);
            float z = (float)((blockZ + 0.5) * Stage5Constants.PromotionBlockM);
            return cell with { LightIndex = CanopyLightEvaluator.LightIndexAt(x, z, woodland) };
        }).ToArray())).ToArray();
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
        public double SeedArrival, StartingAbundance, VegetationLoss, LightLoss, BrowseLoss, PromotionExported;
        public int QualifyingCells, BlockCapDeferredCells;

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
                Promotions.AsReadOnly());
        }
    }
}
