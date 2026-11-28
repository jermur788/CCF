namespace SitkaP1.Core;

/// <summary>The outcome of one Phase 2 regeneration year: both next states, the new adults per area and the year's accounts.</summary>
internal sealed record Phase2AnnualResult(MultiAreaStage5State NextStage5, MultiAreaPhase2State NextPhase2,
    IReadOnlyDictionary<string, IReadOnlyList<AnnualTree>> PromotedTrees, Stage5YearAccount Stage5Account, Phase2YearAccount Phase2Account);

/// <summary>
/// The Phase 2 annual steps 3d–5 (spec §4) for all three species, run after every rack's adult step. It replaces
/// <see cref="Stage5Annual.Step(MultiAreaStage5State, IReadOnlyList{Stage5AreaInput}, int)"/> only at 1/1/1/1/1 and never runs
/// at Species 0. Sitka keeps every Stage 5 rule, constant, stream and account, and its bands stay in the Stage 5 cells; Oak
/// and Beech bands live beside them in the Phase 2 state. With no Oak or Beech adult, band or planted juvenile the Sitka
/// arithmetic, summation order and accounts are exactly Stage 5's (owner Q6, test P2-03.8).
/// <list type="bullet">
/// <item>3d: cell bramble/bracken step (Stage 5 step 5a), then spot-patch recovery (Unity PrepareCompetitionForYear guard).</item>
/// <item>3e: existing bands of every species grow and survive (per-species tables, cohort browse access 1).</item>
/// <item>3f–3g: one independent mast roll per broadleaf; per-species cross-rack seed rain; establishment in ordinal SpeciesId
/// order into ONE shared capacity per cell (Σ density / 1.5·a ≤ 1). The 4-band + accumulator limit is per species in a cell
/// (Unity NormalizeBands(cell, speciesId, origin)).</item>
/// <item>3h: at most ONE natural promotion per aligned 5 m block per year across all species, with the Stage 5 block credit;
/// winner: oldest EstablishYear, tallest, lowest (Column, Row), ordinal SpeciesId. Cohort export takes same-species,
/// same-year sibling bands only.</item>
/// <item>4: planted juveniles (Unity AdvancePlantedJuveniles), after natural growth and promotion; PL-* promotion is exempt
/// from the block cap and does not recompute this year's canopy.</item>
/// </list>
/// It is a pure function of its inputs; areas are matched by ID, so area order cannot change any result.
/// </summary>
internal static class Phase2Annual
{
    /// <summary>Species order in every cell: ordinal SpeciesId ("beech" &lt; "sessile-oak" &lt; "sitka-spruce").</summary>
    private const int SpeciesCount = 3;
    private const int SitkaSlot = 2;

    private static Phase2Species? SlotSpecies(int slot) => slot < SitkaSlot ? Phase2Species.Broadleaves[slot] : null;
    private static string SlotId(int slot) => slot < SitkaSlot ? Phase2Species.Broadleaves[slot].SpeciesId : Phase2Species.SitkaId;

    /// <summary>Phase 2 genesis: an empty broadleaf band list for every Stage 5 cell, and the genesis species map.</summary>
    public static MultiAreaPhase2State Genesis(Phase2Options options, int stage5MastSeed, MultiAreaStage5State stage5,
        IReadOnlyDictionary<string, string> treeSpecies) =>
        new(options, options.OakMastSeed ?? stage5MastSeed, options.BeechMastSeed ?? stage5MastSeed,
            new Dictionary<string, string>(treeSpecies, StringComparer.Ordinal),
            Array.AsReadOnly(stage5.Areas.Select(area => new Phase2AreaBands(area.AreaId, Array.AsReadOnly(area.Cells
                .Select(cell => new Phase2CellBands(cell.Column, cell.Row, Array.Empty<Phase2Band>())).ToArray()))).ToArray()),
            Array.Empty<PlantedJuvenile>(), Array.Empty<BrowseShelter>(), Array.Empty<SpotPatch>(),
            Array.Empty<MultiAreaPlantingOrder>(), Array.Empty<MultiAreaPlantingOrder>(), Array.Empty<Phase2WorkRecord>(), 1, 1, 1,
            Array.Empty<Phase2YearAccount>());

    /// <summary>One Phase 2 year ending in <paramref name="yearAfter"/>, on the model light of the post-adult-step woodland.</summary>
    /// <remarks>
    /// <paramref name="moisture"/> is null (or neutral) on every woodland without a site-moisture class, and the step is then
    /// unchanged. A non-neutral field scales each species' requested natural establishment by its moisture establishment
    /// modifier at the cell centre (SPEC-multi-2d-scots-pine-moisture-V1 Part 2). Scots pine adults are never Sitka seed sources.
    /// </remarks>
    public static Phase2AnnualResult Step(MultiAreaStage5State start5, MultiAreaPhase2State start2, IReadOnlyList<Stage5AreaInput> areas,
        int yearAfter, IReadOnlyList<Phase2DensityRecord> density, SiteMoistureField? moisture = null)
    {
        var inputs = InStateOrder(start5, areas);
        return Step(start5, start2, inputs, yearAfter, density, ModelLight(inputs, start5.Options.GridCellM), moisture);
    }

    /// <summary>The same year with the light field given (tests use it to build controlled light).</summary>
    public static Phase2AnnualResult Step(MultiAreaStage5State start5, MultiAreaPhase2State start2, IReadOnlyList<Stage5AreaInput> areas,
        int yearAfter, IReadOnlyList<Phase2DensityRecord> density, IReadOnlyDictionary<string, IReadOnlyList<CanopyLightCell>> lightByArea,
        SiteMoistureField? moisture = null)
    {
        var establishmentField = moisture is { IsNeutral: false } ? moisture : null;
        var options = start5.Options;
        var scale = Stage5CellScale.For(options.GridCellM);
        var inputs = InStateOrder(start5, areas);
        RequireSameAreas(start5, start2);
        var light = inputs.Select(area => lightByArea.TryGetValue(area.AreaId, out var cells) ? cells
            : throw new P1Refusal("Stage5LightMissing", "the Phase 2 step needs the light of " + area.AreaId)).ToArray();
        var speciesMap = new Dictionary<string, string>(start2.TreeSpecies, StringComparer.Ordinal);
        Phase2Species? BroadleafOf(string treeId) => speciesMap.TryGetValue(treeId, out var id) ? Phase2Species.BroadleafOrNull(id) : null;

        // 3f–3g: the post-adult-step woodland, Sitka's STAGE5-MAST and sources exactly as Stage 5 (Sitka stems only), then one
        // independent mast stream and one source set per broadleaf.
        var woodland = CrownModel.Woodland(inputs.SelectMany(area => area.Trees));
        var sitkaMast = Stage5Mast.ForYear(start5.MastSeed, yearAfter);
        var sitkaSources = Stage5SeedRain.Sources(woodland.Where(tree => BroadleafOf(tree.TreeId) is null &&
            !ScotsPineLive.IsPine(speciesMap.GetValueOrDefault(tree.TreeId))), sitkaMast.Multiplier);
        var broadleafMast = new Stage5MastState[2];
        var broadleafSources = new IReadOnlyList<Stage5SeedSource>[2];
        for (int s = 0; s < 2; s++)
        {
            var species = Phase2Species.Broadleaves[s];
            int seed = species == Phase2Species.Oak ? start2.OakMastSeed : start2.BeechMastSeed;
            broadleafMast[s] = Phase2Mast.ForYear(species, seed, yearAfter);
            broadleafSources[s] = Phase2SeedRain.Sources(woodland.Where(tree => BroadleafOf(tree.TreeId) == species), species,
                broadleafMast[s].Multiplier);
        }

        // Per area, per cell, per species slot: this year's work bands; per area, per slot: the tallies.
        var work = new List<Stage5BandWork>[inputs.Length][][];
        var covers = new (float Bramble, float Bracken)[inputs.Length][];
        var tallies = new Tally[inputs.Length][];
        for (int i = 0; i < inputs.Length; i++)
        {
            var cells = start5.Areas[i].Cells;
            var broadleafCells = start2.Areas[i].Cells;
            RequireSameLattice(cells, light[i], broadleafCells, start5.Areas[i].AreaId);
            work[i] = new List<Stage5BandWork>[cells.Count][];
            covers[i] = new (float, float)[cells.Count];
            tallies[i] = Enumerable.Range(0, SpeciesCount).Select(_ => new Tally(cells.Count)).ToArray();
            for (int c = 0; c < cells.Count; c++)
            {
                var cell = cells[c];
                float cellLight = light[i][c].LightIndex;
                // 3d. Bramble and bracken move toward this year's light target (Stage 5 step 5a).
                float target = Stage5Undergrowth.Target(cellLight);
                float bramble = Stage5Undergrowth.Step(cell.Bramble, target);
                float bracken = Stage5Undergrowth.Step(cell.Bracken, target);
                covers[i][c] = (bramble, bracken);
                // 3e. Existing bands of every species grow and survive before any new establishment.
                var slots = new List<Stage5BandWork>[SpeciesCount];
                for (int s = 0; s < SpeciesCount; s++)
                {
                    var tally = tallies[i][s];
                    var bands = s == SitkaSlot
                        ? cell.Bands.Select(Stage5BandWork.From).ToList()
                        : broadleafCells[c].Bands.Where(band => band.SpeciesId == SlotId(s)).Select(FromBand).ToList();
                    GrowAndSurvive(SlotSpecies(s), bands, cellLight, bramble, bracken, options.BackgroundBrowsePressure, yearAfter, tally);
                    Stage5Bands.Normalize(bands, scale, tally.Bands);
                    slots[s] = bands;
                }
                // 3g. Seed rain at the cell centre and establishment, species in ordinal order, into one shared capacity.
                for (int s = 0; s < SpeciesCount; s++)
                {
                    var tally = tallies[i][s];
                    float seedRain, requested, initialHeight;
                    if (s == SitkaSlot)
                    {
                        seedRain = Stage5SeedRain.SeedRainAt(cell.CentreXM, cell.CentreZM, sitkaSources);
                        requested = Stage5Bands.RequestedEstablishment(seedRain, cellLight, scale);
                        initialHeight = Stage5Constants.RegenInitialHeightM;
                    }
                    else
                    {
                        var species = Phase2Species.Broadleaves[s];
                        seedRain = Phase2SeedRain.SeedRainAt(species, cell.CentreXM, cell.CentreZM, broadleafSources[s]);
                        requested = Phase2Juvenile.RequestedEstablishment(species, seedRain, cellLight, scale);
                        initialHeight = species.RegenInitialHeightM;
                    }
                    if (seedRain > 0f) tally.SeedArrival += seedRain;
                    if (establishmentField is not null && requested > 0f)
                        requested *= establishmentField.EstablishmentMultiplier(SlotId(s), cell.CentreXM, cell.CentreZM);
                    if (requested > 0f)
                        tally.CellAccepted[c] = AdmitRecruitment(slots, s, yearAfter, initialHeight, requested, scale, tally.Bands);
                }
                work[i][c] = slots;
            }
        }

        // 3h. Natural promotion under one shared block cap and credit.
        var promoted = inputs.ToDictionary(area => area.AreaId, _ => new List<AnnualTree>(), StringComparer.Ordinal);
        var existingIds = inputs.SelectMany(area => area.Trees).Select(tree => tree.TreeId).ToHashSet(StringComparer.Ordinal);
        var nextBlocks = Promote(start5, inputs, light, work, woodland, scale, yearAfter, tallies, promoted, existingIds, speciesMap);

        // 3d (patches): each spot patch steps toward the target of the cell containing its centre, once per later year.
        var locate = Locator(start5);
        var patches = start2.Patches.Select(patch =>
        {
            if (yearAfter <= patch.UpdatedYear) return patch;
            var (a, c) = locate(patch.XM, patch.ZM) ?? throw new P1Refusal("Phase2PatchOutsideGround", "a spot patch lies outside every ground cell");
            float target = Stage5Undergrowth.Target(light[a][c].LightIndex);
            return patch with
            {
                Bramble = Stage5Undergrowth.Step(patch.Bramble, target),
                Bracken = Stage5Undergrowth.Step(patch.Bracken, target),
                UpdatedYear = yearAfter,
            };
        }).ToArray();

        // 4. Planted juveniles, after adult mortality and after natural growth and promotion.
        var seedOf = inputs.ToDictionary(area => area.AreaId, area => area.SimulationSeed, StringComparer.Ordinal);
        var (juveniles, plantedAccounts) = AdvancePlantedJuveniles(start2, patches, covers, light, locate, woodland, yearAfter,
            options.BackgroundBrowsePressure, seedOf, promoted, existingIds, speciesMap);

        // 5. Next states and accounts.
        var nextAreas5 = new Stage5AreaCells[inputs.Length];
        var nextAreas2 = new Phase2AreaBands[inputs.Length];
        var accounts5 = new Stage5AreaAccount[inputs.Length];
        var natural = new List<Phase2SpeciesAreaAccount>();
        for (int i = 0; i < inputs.Length; i++)
        {
            var cells = start5.Areas[i].Cells;
            var next5 = new Stage5Cell[cells.Count];
            var next2 = new Phase2CellBands[cells.Count];
            for (int c = 0; c < cells.Count; c++)
            {
                next5[c] = cells[c] with
                {
                    Light = light[i][c].LightIndex,
                    Bramble = covers[i][c].Bramble,
                    Bracken = covers[i][c].Bracken,
                    Bands = Array.AsReadOnly(work[i][c][SitkaSlot].Select(band => band.Freeze()).ToArray()),
                };
                var broadleafBands = new List<Phase2Band>();
                for (int s = 0; s < SitkaSlot; s++)
                    broadleafBands.AddRange(work[i][c][s].Select(band => FreezeBand(SlotId(s), band)));
                next2[c] = new Phase2CellBands(cells[c].Column, cells[c].Row, broadleafBands.AsReadOnly());
            }
            nextAreas5[i] = new Stage5AreaCells(start5.Areas[i].AreaId, Array.AsReadOnly(next5));
            nextAreas2[i] = new Phase2AreaBands(start5.Areas[i].AreaId, Array.AsReadOnly(next2));
            accounts5[i] = tallies[i][SitkaSlot].ToStage5Account(start5.Areas[i].AreaId, next5, work[i], scale);
            for (int s = 0; s < SitkaSlot; s++)
                natural.Add(tallies[i][s].ToBroadleafAccount(start5.Areas[i].AreaId, SlotId(s), work[i], s));
        }
        var account5 = new Stage5YearAccount(yearAfter - 1, yearAfter, sitkaMast.Roll, sitkaMast.Label, sitkaMast.Multiplier,
            Array.AsReadOnly(accounts5));
        var mastRecords = Enumerable.Range(0, 2).Select(s => new Phase2MastRecord(Phase2Species.Broadleaves[s].SpeciesId,
            Phase2Species.Broadleaves[s] == Phase2Species.Oak ? start2.OakMastSeed : start2.BeechMastSeed,
            broadleafMast[s].Roll, broadleafMast[s].Label, broadleafMast[s].Multiplier)).ToArray();
        var account2 = new Phase2YearAccount(yearAfter - 1, yearAfter, Array.AsReadOnly(mastRecords), natural.AsReadOnly(), plantedAccounts,
            Array.AsReadOnly(density.OrderBy(record => record.AreaId, StringComparer.Ordinal).ToArray()));

        var nextStage5 = start5 with
        {
            Areas = Array.AsReadOnly(nextAreas5),
            Accounts = Array.AsReadOnly(start5.Accounts.Append(account5).ToArray()),
            Blocks = Array.AsReadOnly(nextBlocks),
        };
        var nextPhase2 = start2 with
        {
            TreeSpecies = speciesMap,
            Areas = Array.AsReadOnly(nextAreas2),
            Juveniles = juveniles,
            Patches = Array.AsReadOnly(patches),
            Accounts = Array.AsReadOnly(start2.Accounts.Append(account2).ToArray()),
        };
        return new Phase2AnnualResult(nextStage5, nextPhase2,
            promoted.ToDictionary(pair => pair.Key, pair => (IReadOnlyList<AnnualTree>)pair.Value.OrderBy(t => t.TreeId, StringComparer.Ordinal)
                .ToArray(), StringComparer.Ordinal), account5, account2);
    }

    // ------------------------------------------------------------------------------------------------ 3e growth and survival

    // Stage5Annual.GrowAndSurvive for Sitka (species null), the same expression set with the broadleaf tables otherwise. Browse
    // probability and p_veg use the band's pre-growth height; cohort access is 1 (shelters never apply to cohorts).
    private static void GrowAndSurvive(Phase2Species? species, List<Stage5BandWork> bands, float light, float bramble, float bracken,
        float pressure, int yearAfter, Tally tally)
    {
        foreach (var band in bands)
        {
            if (band.Density <= 0f) continue;
            float preGrowthHeight = band.Height;
            float browsed = pressure > 0f
                ? species is null ? Stage5Juvenile.BrowseProbability(pressure, preGrowthHeight)
                    : Phase2Juvenile.BrowseProbability(species, pressure, preGrowthHeight, 1f)
                : 0f;
            band.LastBrowsedFraction = browsed;
            band.LastBrowseAssessmentYear = yearAfter;
            tally.StartingAbundance += band.Density;
            float vegetationLoss = Stage5Undergrowth.VegetationLossProbability(bramble, bracken, preGrowthHeight);
            band.Height = species is null ? Stage5Juvenile.GrowHeight(preGrowthHeight, light, browsed)
                : Phase2Juvenile.GrowHeight(species, preGrowthHeight, light, browsed);
            float before = band.Density;
            float lightOnly, lightAndBrowse;
            if (species is null)
            {
                lightOnly = (float)(before * Stage5Juvenile.LightSurvival(light));
                lightAndBrowse = (float)(before * Stage5Juvenile.LightBrowseSurvival(light, browsed));
                band.Density = (float)(before * Stage5Juvenile.Survival(light, browsed, vegetationLoss));
            }
            else
            {
                lightOnly = (float)(before * Phase2Juvenile.LightSurvival(species, light));
                lightAndBrowse = (float)(before * Phase2Juvenile.LightBrowseSurvival(species, light, browsed));
                band.Density = (float)(before * Phase2Juvenile.Survival(species, light, browsed, vegetationLoss));
            }
            tally.LightLoss += before - lightOnly;
            tally.BrowseLoss += lightOnly - lightAndBrowse;
            tally.VegetationLoss += lightAndBrowse - band.Density;
        }
    }

    // ------------------------------------------------------------------------------------------------ 3g shared capacity

    /// <summary>
    /// Shared relative occupancy of one cell: Σ over every species (ordinal order) of density / max(0.01, 1.5·a). With only
    /// Sitka bands this is Stage5Bands.SharedOccupancy exactly (same terms, same order).
    /// </summary>
    internal static float SharedOccupancy(IReadOnlyList<List<Stage5BandWork>> slots, Stage5CellScale scale)
    {
        float occupancy = 0f;
        foreach (var bands in slots)
            foreach (var band in bands)
                if (band.Density > 0f) occupancy += band.Density / MathF.Max(Stage5Constants.CapacityFloor, scale.Capacity);
        return occupancy;
    }

    /// <summary>
    /// Stage5Bands.AdmitRecruitment for species slot <paramref name="slot"/>, with the free capacity taken from the SHARED
    /// occupancy of every species in the cell (Unity ForestEcologyCell.AdmitDensity). Combining, the accumulator and the band
    /// invariants act on this species' own bands only.
    /// </summary>
    internal static float AdmitRecruitment(IReadOnlyList<List<Stage5BandWork>> slots, int slot, int establishYear, float height,
        float requested, Stage5CellScale scale, Stage5BandCounters counters)
    {
        if (requested <= 0f) return 0f;
        var bands = slots[slot];
        var recruit = new Stage5BandWork { Density = 0f, Height = MathF.Max(0f, height), EstablishYear = establishYear, OriginYear = establishYear };
        float free = MathF.Max(0f, 1f - SharedOccupancy(slots, scale));
        float accepted = MathF.Min(requested, free * MathF.Max(Stage5Constants.CapacityFloor, scale.Capacity));
        if (accepted > 0f) recruit.Density += accepted;
        else accepted = 0f;
        counters.EstablishmentRequested += requested;
        counters.EstablishmentAccepted += accepted;
        counters.CapacityRejected += requested - accepted;
        if (accepted <= 0f) return 0f;

        var sameYear = bands.Find(band => band.EstablishYear == establishYear);
        var accumulator = bands.Find(band => band.Density < scale.Threshold);
        if (sameYear is not null)
            Combine(bands, sameYear, recruit, scale, counters);
        else if (accepted < scale.Threshold && accumulator is not null)
            Combine(bands, accumulator, recruit, scale, counters);
        else
        {
            Stage5Bands.InsertBand(bands, recruit);
            counters.BandsCreated++;
        }
        Stage5Bands.Normalize(bands, scale, counters);
        return accepted;
    }

    // Stage5Bands.CombineBands (private there): a sub-threshold record that ends at or above the threshold is a crossing.
    private static void Combine(List<Stage5BandWork> bands, Stage5BandWork a, Stage5BandWork b, Stage5CellScale scale, Stage5BandCounters counters)
    {
        bool hadSubThreshold = a.Density < scale.Threshold || b.Density < scale.Threshold;
        var merged = Stage5Bands.MergeBands(bands, a, b);
        if (hadSubThreshold && merged.Density >= scale.Threshold) counters.ThresholdCrossings++;
    }

    // ------------------------------------------------------------------------------------------------ 3h promotion

    /// <summary>One cell's qualifying band of one species.</summary>
    internal sealed record Candidate(int AreaOrder, int CellIndex, int Slot, string SpeciesId, int Column, int Row, int BlockX, int BlockZ,
        int EstablishYear, float Height);

    /// <summary>
    /// The one winner of every block (fixed block order): oldest EstablishYear, tallest, lowest Column, lowest Row, ordinal
    /// SpeciesId (owner decision 2). With one species this is Stage5Promotion.Winners exactly.
    /// </summary>
    internal static IReadOnlyList<Candidate> Winners(IEnumerable<Candidate> candidates) =>
        candidates
            .GroupBy(candidate => (candidate.BlockX, candidate.BlockZ))
            .OrderBy(group => group.Key.BlockX).ThenBy(group => group.Key.BlockZ)
            .Select(group => group
                .OrderBy(c => c.EstablishYear)
                .ThenByDescending(c => c.Height)
                .ThenBy(c => c.Column)
                .ThenBy(c => c.Row)
                .ThenBy(c => c.SpeciesId, StringComparer.Ordinal)
                .First())
            .ToArray();

    private static Stage5BandWork? OldestQualifyingBand(int slot, List<Stage5BandWork> bands, float light, Stage5CellScale scale)
    {
        if (slot == SitkaSlot) return Stage5Promotion.OldestQualifyingBand(bands, light, scale);
        var species = Phase2Species.Broadleaves[slot];
        Stage5BandWork? oldest = null;
        foreach (var band in bands)
        {
            if (band.Density < scale.Threshold || !Phase2Juvenile.CanPromote(species, band.Height, light)) continue;
            if (oldest is null || band.EstablishYear < oldest.EstablishYear) oldest = band;
        }
        return oldest;
    }

    // Stage5Annual.Promote with every species' candidates competing for one block credit (spec §6.4, Stage 5 Option A).
    private static Stage5BlockCredit[] Promote(MultiAreaStage5State start, Stage5AreaInput[] inputs, IReadOnlyList<CanopyLightCell>[] light,
        List<Stage5BandWork>[][][] work, AnnualTree[] woodland, Stage5CellScale scale, int yearAfter, Tally[][] tallies,
        Dictionary<string, List<AnnualTree>> promoted, HashSet<string> existingIds, Dictionary<string, string> speciesMap)
    {
        double cellM = start.Options.GridCellM;
        int fullBlock = Stage5Promotion.CellsPerBlock(cellM);
        var blockCells = new Dictionary<(int X, int Z), List<(int Area, int Cell)>>();
        var candidates = new List<Candidate>();
        for (int i = 0; i < inputs.Length; i++)
        {
            var cells = start.Areas[i].Cells;
            for (int c = 0; c < cells.Count; c++)
            {
                var block = Stage5Promotion.BlockOf(cells[c].Column, cells[c].Row, cellM);
                if (!blockCells.TryGetValue(block, out var members)) blockCells[block] = members = new List<(int, int)>();
                members.Add((i, c));
                for (int s = 0; s < SpeciesCount; s++)
                {
                    var band = OldestQualifyingBand(s, work[i][c][s], light[i][c].LightIndex, scale);
                    if (band is null || band.Density <= 0f) continue;
                    candidates.Add(new Candidate(i, c, s, SlotId(s), cells[c].Column, cells[c].Row, block.X, block.Z, band.EstablishYear, band.Height));
                    tallies[i][s].QualifyingCells++;
                }
            }
        }

        var credit = new Dictionary<(int X, int Z), int>(start.Blocks.Count);
        foreach (var block in start.Blocks) credit.Add((block.BlockX, block.BlockZ), checked(block.Credit + block.InPolygonCells));

        var bandsGone = new HashSet<(int Area, int Cell, int Slot, int EstablishYear)>();
        foreach (var winner in Winners(candidates))
        {
            var key = (winner.BlockX, winner.BlockZ);
            if (!credit.TryGetValue(key, out int available))
                throw new InvalidOperationException("Phase 2 promotion block without a credit record.");
            if (available < fullBlock)
            {
                tallies[winner.AreaOrder][winner.Slot].BudgetWithheldBlocks++;
                continue;
            }
            credit[key] = available - fullBlock;

            var input = inputs[winner.AreaOrder];
            var cell = start.Areas[winner.AreaOrder].Cells[winner.CellIndex];
            var bands = work[winner.AreaOrder][winner.CellIndex][winner.Slot];
            var band = OldestQualifyingBand(winner.Slot, bands, light[winner.AreaOrder][winner.CellIndex].LightIndex, scale)!;
            string treeId = Stage5Promotion.TreeIdFor(yearAfter, input.AreaId, cell.Column, cell.Row);
            if (!existingIds.Add(treeId))
                throw new P1Refusal("Stage5TreeIdNotUnique", "a promoted tree ID already exists: " + treeId);
            var (x, z, fallback) = Stage5Promotion.Place(input.Polygon, cell.CentreXM, cell.CentreZM, cellM, input.AreaId,
                cell.Column, cell.Row, yearAfter, input.SimulationSeed);
            var (tree, ci) = SlotSpecies(winner.Slot) is { } broadleaf
                ? NewBroadleafAdult(broadleaf, treeId, x, z, Math.Max(1, yearAfter - band.EstablishYear), band.Height, woodland)
                : Stage5Promotion.NewAdult(treeId, x, z, yearAfter, band.EstablishYear, band.Height, woodland);
            if (SlotSpecies(winner.Slot) is { } promotedSpecies) speciesMap[treeId] = promotedSpecies.SpeciesId;
            bands.Remove(band);
            bandsGone.Add((winner.AreaOrder, winner.CellIndex, winner.Slot, band.EstablishYear));
            promoted[input.AreaId].Add(tree);
            tallies[winner.AreaOrder][winner.Slot].PromotionExported += band.Density;

            int cohortBands = 0;
            double cohortDensity = 0.0;
            foreach (var (siblingArea, siblingCell) in blockCells[key])
            {
                if (siblingArea == winner.AreaOrder && siblingCell == winner.CellIndex) continue;
                var sibling = Stage5Promotion.CohortBand(work[siblingArea][siblingCell][winner.Slot], band.EstablishYear, scale);
                if (sibling is null) continue;
                work[siblingArea][siblingCell][winner.Slot].Remove(sibling);
                bandsGone.Add((siblingArea, siblingCell, winner.Slot, sibling.EstablishYear));
                cohortBands++;
                cohortDensity += sibling.Density;
                tallies[siblingArea][winner.Slot].PromotionExported += sibling.Density;
                tallies[siblingArea][winner.Slot].CohortBandsExported++;
            }
            tallies[winner.AreaOrder][winner.Slot].Promotions.Add(new Stage5PromotionRecord(treeId, input.AreaId, cell.Column, cell.Row,
                winner.BlockX, winner.BlockZ, band.EstablishYear, tree.AgeYears, tree.HeightM, tree.DbhCm, tree.CrownRadiusM, ci,
                tree.XM, tree.ZM, fallback, band.Density, cohortBands, cohortDensity));
        }
        foreach (var candidate in candidates)
            if (!bandsGone.Contains((candidate.AreaOrder, candidate.CellIndex, candidate.Slot, candidate.EstablishYear)))
                tallies[candidate.AreaOrder][candidate.Slot].BlockCapDeferredCells++;

        return start.Blocks.Select(block => block with { Credit = Math.Min(credit[(block.BlockX, block.BlockZ)], fullBlock) }).ToArray();
    }

    /// <summary>
    /// A new broadleaf adult (natural or planted): DBH = clamp(1.5·H, 2, 20) cm and r_0 = the species' C2 target at the
    /// connected-woodland CI of the post-adult-step woodland (not open-grown r_pot; spec §3.2, §6.4, §7.3).
    /// </summary>
    internal static (AnnualTree Tree, float WoodlandCi) NewBroadleafAdult(Phase2Species species, string treeId, float xM, float zM, int ageYears,
        float heightM, AnnualTree[] woodland)
    {
        float dbh = Math.Clamp(heightM * Stage5Constants.PromotionDbhCmPerMetreHeight, Stage5Constants.PromotionDbhMinCm,
            Stage5Constants.PromotionDbhMaxCm);
        var probe = new AnnualTree(treeId, xM, zM, ageYears, dbh, heightM, 0f);
        float ci = CrownModel.WoodlandCompetition(probe, woodland).Ci;
        return (probe with { CrownRadiusM = Phase2Crown.TargetRadiusM(species, dbh, ci) }, ci);
    }

    // ------------------------------------------------------------------------------------------------ 4 planted juveniles

    /// <summary>
    /// The exposure (max of bramble and bracken) at an exact point: the newest spot patch containing it, else its cell's covers.
    /// </summary>
    internal static float ExposureAt(IReadOnlyList<SpotPatch> patches, float xM, float zM, (float Bramble, float Bracken) cellCovers)
    {
        for (int p = patches.Count - 1; p >= 0; p--)
            if (patches[p].Contains(xM, zM)) return MathF.Max(patches[p].Bramble, patches[p].Bracken);
        return MathF.Max(cellCovers.Bramble, cellCovers.Bracken);
    }

    /// <summary>Binary shelter access (Unity BrowsingConditions): 0 if an effective shelter lies within 0.25 m, else 1.</summary>
    internal static float AccessAt(IReadOnlyList<BrowseShelter> shelters, float xM, float zM, int year)
    {
        const float r = Phase2Constants.ShelterMatchRadiusM;
        foreach (var shelter in shelters)
        {
            float dx = shelter.XM - xM, dz = shelter.ZM - zM;
            if (dx * dx + dz * dz > r * r) continue;
            if (shelter.IsEffective(year)) return 0f;
        }
        return 1f;
    }

    // Unity ScenarioOneManager.AdvancePlantedJuveniles (regeneration model 2), in ordinal juvenileId order.
    private static (IReadOnlyList<PlantedJuvenile>, IReadOnlyList<Phase2PlantedAccount>) AdvancePlantedJuveniles(MultiAreaPhase2State start,
        IReadOnlyList<SpotPatch> patches, (float Bramble, float Bracken)[][] covers, IReadOnlyList<CanopyLightCell>[] light,
        Func<float, float, (int Area, int Cell)?> locate, AnnualTree[] woodland, int yearAfter, float pressure,
        Dictionary<string, int> seedOf, Dictionary<string, List<AnnualTree>> promoted, HashSet<string> existingIds,
        Dictionary<string, string> speciesMap)
    {
        var accounts = Phase2Species.Broadleaves.ToDictionary(s => s.SpeciesId, _ => new PlantedTally(), StringComparer.Ordinal);
        var next = start.Juveniles.ToDictionary(j => j.JuvenileId, StringComparer.Ordinal);
        foreach (var juvenile in start.Juveniles.Where(j => j.IsActive).OrderBy(j => j.JuvenileId, StringComparer.Ordinal))
        {
            var species = Phase2Species.BroadleafOrNull(juvenile.SpeciesId);
            if (species is null || locate(juvenile.XM, juvenile.ZM) is not var (a, c)) continue;
            var tally = accounts[species.SpeciesId];
            tally.Starting++;
            int seed = seedOf[juvenile.AreaId];
            float cellLight = light[a][c].LightIndex;
            float age = juvenile.AgeYears + 1f;
            float height = juvenile.HeightM;
            // 2. Vegetation loss at the exact point, pre-growth height.
            float exposure = ExposureAt(patches, juvenile.XM, juvenile.ZM, covers[a][c]);
            float vegetation = Stage5Undergrowth.VegetationLossProbability(exposure, exposure, height);
            if (vegetation > 0f && Phase2Random.Roll(Phase2Random.JuvenileVegetationKey(juvenile.JuvenileId), yearAfter, seed) < vegetation)
            {
                next[juvenile.JuvenileId] = juvenile with
                {
                    AgeYears = age, Alive = false, DeathYear = yearAfter, DeathCause = "vegetation",
                    LastVegetationLossProbability = vegetation, LastAssessmentYear = yearAfter,
                };
                tally.VegetationDeaths++;
                continue;
            }
            // 3. Browse at the pre-growth height with binary shelter access; one deterministic event.
            float access = AccessAt(start.Shelters, juvenile.XM, juvenile.ZM, yearAfter);
            float probability = Phase2Juvenile.BrowseProbability(species, pressure, height, access);
            bool browsed = probability > 0f &&
                Phase2Random.Roll(Phase2Random.JuvenileBrowseKey(juvenile.JuvenileId), yearAfter, seed) < probability;
            // 4. Growth with the containing cell's light.
            height = Phase2Juvenile.GrowHeight(species, height, cellLight, browsed ? 1f : 0f);
            // 5. Survival.
            float roll = Phase2Random.Roll(Phase2Random.JuvenileKey(juvenile.JuvenileId), yearAfter, seed);
            bool survives = browsed
                ? roll < (float)Phase2Juvenile.LightBrowseSurvival(species, cellLight, 1f)
                : roll < (float)Phase2Juvenile.LightSurvival(species, cellLight);
            var updated = juvenile with
            {
                AgeYears = age, HeightM = height, LastYearBrowsed = browsed, LastBrowseProbability = probability,
                LastVegetationLossProbability = vegetation, LastAssessmentYear = yearAfter,
            };
            if (!survives)
            {
                bool lightDeath = roll >= Phase2Juvenile.LightSurvival(species, cellLight);
                next[juvenile.JuvenileId] = updated with { Alive = false, DeathYear = yearAfter, DeathCause = lightDeath ? "light" : "browse" };
                if (lightDeath) tally.LightDeaths++; else tally.BrowseDeaths++;
                continue;
            }
            tally.Remaining++;
            // 6. Promotion at the exact X/Z (block cap exempt; no same-year canopy recompute).
            if (Phase2Juvenile.CanPromote(species, height, cellLight))
            {
                string treeId = "PL-" + juvenile.JuvenileId;
                if (!existingIds.Add(treeId)) throw new P1Refusal("Stage5TreeIdNotUnique", "a promoted tree ID already exists: " + treeId);
                int treeAge = Math.Max(1, (int)Math.Round(age, MidpointRounding.ToEven));
                var (tree, _) = NewBroadleafAdult(species, treeId, juvenile.XM, juvenile.ZM, treeAge, height, woodland);
                promoted[juvenile.AreaId].Add(tree);
                speciesMap[treeId] = species.SpeciesId;
                updated = updated with { PromotedTreeId = treeId };
                tally.Remaining--;
                tally.Promoted++;
                tally.PromotedIds.Add(treeId);
            }
            next[juvenile.JuvenileId] = updated;
        }
        var juveniles = start.Juveniles.Select(j => next[j.JuvenileId]).ToArray();
        var accountList = accounts.Select(pair => new Phase2PlantedAccount(pair.Key, pair.Value.Starting, pair.Value.VegetationDeaths,
            pair.Value.LightDeaths, pair.Value.BrowseDeaths, pair.Value.Remaining, pair.Value.Promoted,
            pair.Value.PromotedIds.AsReadOnly())).OrderBy(account => account.SpeciesId, StringComparer.Ordinal).ToArray();
        return (Array.AsReadOnly(juveniles), Array.AsReadOnly(accountList));
    }

    /// <summary>
    /// The ground cell (area order, cell index) containing a point: the lattice cell floor(x/cell), floor(z/cell) of whichever
    /// area holds it (cells of different areas never coincide), or null when no area has that cell.
    /// </summary>
    internal static Func<float, float, (int Area, int Cell)?> Locator(MultiAreaStage5State state)
    {
        double cellM = state.Options.GridCellM;
        var index = new Dictionary<(int, int), (int, int)>();
        for (int i = 0; i < state.Areas.Count; i++)
            for (int c = 0; c < state.Areas[i].Cells.Count; c++)
                index[(state.Areas[i].Cells[c].Column, state.Areas[i].Cells[c].Row)] = (i, c);
        return (x, z) => index.TryGetValue(((int)Math.Floor(x / cellM), (int)Math.Floor(z / cellM)), out var hit) ? hit : null;
    }

    // ------------------------------------------------------------------------------------------------ helpers

    private static Stage5BandWork FromBand(Phase2Band band) => new()
    {
        Density = band.Density, Height = band.Height, EstablishYear = band.EstablishYear, OriginYear = band.OriginYear,
        LastBrowsedFraction = band.LastBrowsedFraction, LastBrowseAssessmentYear = band.LastBrowseAssessmentYear,
    };

    private static Phase2Band FreezeBand(string speciesId, Stage5BandWork band) => new(speciesId, Phase2Band.Natural, band.Density,
        band.Height, band.EstablishYear, band.OriginYear, band.LastBrowsedFraction, band.LastBrowseAssessmentYear);

    private static Stage5AreaInput[] InStateOrder(MultiAreaStage5State start, IReadOnlyList<Stage5AreaInput> areas)
    {
        var byId = areas.ToDictionary(area => area.AreaId, StringComparer.Ordinal);
        if (byId.Count != start.Areas.Count || start.Areas.Any(area => !byId.ContainsKey(area.AreaId)))
            throw new P1Refusal("Stage5AreasMismatch", "the Phase 2 step needs exactly the areas of its state");
        return start.Areas.Select(area => byId[area.AreaId]).ToArray();
    }

    private static void RequireSameAreas(MultiAreaStage5State start5, MultiAreaPhase2State start2)
    {
        if (start5.Areas.Count != start2.Areas.Count ||
            start5.Areas.Where((area, i) => area.AreaId != start2.Areas[i].AreaId || area.Cells.Count != start2.Areas[i].Cells.Count).Any())
            throw new P1Refusal("Phase2LatticeChanged", "the Phase 2 band lattice no longer matches the Stage 5 cells");
    }

    private static IReadOnlyDictionary<string, IReadOnlyList<CanopyLightCell>> ModelLight(IReadOnlyList<Stage5AreaInput> areas, double gridCellM)
    {
        var canopyAreas = areas.Select(area => new CanopyArea(area.AreaId,
            Array.AsReadOnly(area.Polygon.Select(v => new CanopyVertex(v.XM, v.ZM)).ToArray()), area.AreaHa, area.Trees)).ToArray();
        var fields = CanopyLightEvaluator.LightFields(canopyAreas, gridCellM);
        return areas.Select((area, i) => (area.AreaId, fields[i].Cells))
            .ToDictionary(pair => pair.AreaId, pair => pair.Cells, StringComparer.Ordinal);
    }

    private static void RequireSameLattice(IReadOnlyList<Stage5Cell> cells, IReadOnlyList<CanopyLightCell> light,
        IReadOnlyList<Phase2CellBands> broadleaf, string areaId)
    {
        if (cells.Count != light.Count || cells.Count != broadleaf.Count)
            throw new P1Refusal("Stage5LatticeChanged", "the light lattice of " + areaId + " no longer matches its Stage 5 cells");
        for (int c = 0; c < cells.Count; c++)
            if (cells[c].Column != light[c].Column || cells[c].Row != light[c].Row ||
                cells[c].Column != broadleaf[c].Column || cells[c].Row != broadleaf[c].Row)
                throw new P1Refusal("Stage5LatticeChanged", "the light lattice of " + areaId + " no longer matches its Stage 5 cells");
    }

    /// <summary>One species' running totals in one area while a year is built (diagnostics only).</summary>
    private sealed class Tally
    {
        public readonly Stage5BandCounters Bands = new();
        public readonly List<Stage5PromotionRecord> Promotions = new();
        public readonly float[] CellAccepted;
        public double SeedArrival, StartingAbundance, VegetationLoss, LightLoss, BrowseLoss, PromotionExported;
        public int QualifyingCells, BlockCapDeferredCells, CohortBandsExported, BudgetWithheldBlocks;

        public Tally(int cells) => CellAccepted = new float[cells];

        // Stage5Annual.AreaTally.ToAccount for the Sitka slot. Relative occupancy is the cell's SHARED occupancy (every
        // species); with Sitka only it is the Stage 5 value exactly.
        public Stage5AreaAccount ToStage5Account(string areaId, Stage5Cell[] cells, List<Stage5BandWork>[][] work, Stage5CellScale scale)
        {
            int n = cells.Length;
            double ending = cells.Sum(cell => cell.Abundance);
            double occupancy = work.Sum(slots => (double)SharedOccupancy(slots, scale));
            return new Stage5AreaAccount(areaId, n, SeedArrival, StartingAbundance, VegetationLoss, LightLoss, BrowseLoss,
                Bands.EstablishmentRequested, Bands.EstablishmentAccepted, Bands.CapacityRejected, Bands.BandsCreated,
                Bands.BandMerges, Bands.ThresholdCrossings, QualifyingCells, BlockCapDeferredCells, PromotionExported, ending,
                n == 0 ? 0.0 : occupancy / n, n == 0 ? 0.0 : cells.Average(cell => (double)cell.Bramble),
                n == 0 ? 0.0 : cells.Average(cell => (double)cell.Bracken), n == 0 ? 0.0 : cells.Average(cell => (double)cell.Light),
                Promotions.AsReadOnly(), CohortBandsExported, BudgetWithheldBlocks, Array.AsReadOnly(CellAccepted));
        }

        public Phase2SpeciesAreaAccount ToBroadleafAccount(string areaId, string speciesId, List<Stage5BandWork>[][] work, int slot) =>
            new(areaId, speciesId, SeedArrival, StartingAbundance, VegetationLoss, LightLoss, BrowseLoss, Bands.EstablishmentRequested,
                Bands.EstablishmentAccepted, Bands.CapacityRejected, Bands.BandsCreated, Bands.BandMerges, Bands.ThresholdCrossings,
                QualifyingCells, BlockCapDeferredCells, PromotionExported, work.Sum(slots => slots[slot].Sum(band => (double)band.Density)),
                CohortBandsExported, BudgetWithheldBlocks, Promotions.AsReadOnly());
    }

    private sealed class PlantedTally
    {
        public int Starting, VegetationDeaths, LightDeaths, BrowseDeaths, Remaining, Promoted;
        public readonly List<string> PromotedIds = new();
    }
}
