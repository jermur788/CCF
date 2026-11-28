using SitkaP1.Core.SpeciesWave2;
using SitkaP1.Core.SpeciesWave2.ScotsPine;

namespace SitkaP1.Core;

// Scots pine Part 2 live wiring (SPEC-multi-2d-scots-pine-moisture-V1, Option A). The Part 1 engine owns the Scots pine card,
// growth domain and increment, the regeneration gates and seed-year schedule, and the timber grades, prices and density; this
// file only connects them to map state. The few values declared below are the live-loop glue Part 1 deliberately left to
// Part 2 (seed search radius, recruit odds, juvenile growth, promotion size, stocking threshold, maturity). Every one is a
// provisional [Calibration] teaching value, not an Irish measurement (D-059).

/// <summary>Live-loop identity and [Calibration] glue values for Scots pine.</summary>
internal static class ScotsPineLive
{
    /// <summary>The live species ID ("scots-pine"), the same string as the Part 1 table and card.</summary>
    public const string SpeciesId = SpeciesMoistureTable.ScotsPine;

    public static SpeciesProfileCard Profile => ScotsPineEngine.Profile;

    // ---- Local seed and natural recruitment [Calibration] ----
    /// <summary>A mature on-map pine within this distance of a gap cell centre counts as local seed.</summary>
    public const double SeedTreeSearchRadiusM = 30.0;
    /// <summary>Chance that one eligible gap cell gains one recruit in a good seed year, before the site establishment modifier.</summary>
    public const float BaseRecruitProbability = 0.25f;
    public const float RecruitInitialHeightM = 0.10f;

    // ---- Juveniles (planted and natural) [Calibration] ----
    public const float JuvenileHeightGrowthMPerYear = 0.40f;
    /// <summary>A light-demanding juvenile dies in a cell darker than this.</summary>
    public const float JuvenileMinimumLight = 0.20f;
    /// <summary>Unsheltered juveniles below this height lose growth to browsing in proportion to the browse pressure.</summary>
    public const float BrowseEscapeHeightM = 1.5f;
    /// <summary>Juveniles shorter than this die when bramble/bracken exposure at their point reaches <see cref="SmotheringExposure"/>.</summary>
    public const float SmotheringHeightM = 1.0f;
    public const float SmotheringExposure = 0.70f;
    public const float PromotionHeightM = 3.0f;

    // ---- Adults [Calibration] ----
    /// <summary>Generic-occupancy growing-space weight; no pine-specific ordinal class has been researched, so the reference weight is used.</summary>
    public const double GrowingSpaceWeight = 1.0;
    /// <summary>A pine promoted where its lane holds at least this many living stems per hectare records tight early stocking.</summary>
    public const double TightEarlyStockingStemsPerHa = 2_500.0;
    /// <summary>Harvest maturity for the Sawlog grade.</summary>
    public const int MatureAgeYears = 40;
    /// <summary>Smallest DBH that can fall back to Small Roundwood rather than Pulp/Chip.</summary>
    public const float SmallRoundwoodMinimumDbhCm = 14f;

    /// <summary>Pruning lifts: 2.5 m, then 5.5 m (inside the card's 5–6 m clear-bole target).</summary>
    public static readonly IReadOnlyList<float> LiftTargetHeightsM = Array.AsReadOnly(new[] { 2.5f, 5.5f });
    public const int MaxLifts = 2;

    public const string Evidence =
        "Scots pine: card, GROWFOR growth domain, DBH increment, seed-year schedule, gap thresholds, timber grades, prices " +
        "(Sawlog EUR 68/t, Small Roundwood EUR 45/t, Pulp/Chip EUR 38/t), 1.02 t/m3 and EUR 0.45 sapling are Part 1 values; " +
        "the 30 m seed search, 25 % recruit odds, 0.40 m/yr juvenile growth, 3.0 m promotion, 2,500 stems/ha tight stocking, " +
        "40-year maturity, 2.5/5.5 m lifts and the shared Stage 4 height curve are live-loop glue. All are provisional " +
        "[Calibration] teaching estimates, not Irish yield predictions.";

    public static bool IsPine(string? speciesId) => string.Equals(speciesId, SpeciesId, StringComparison.Ordinal);

    public static string RecruitIdFor(int number) => "SPR" + number.ToString("0000", System.Globalization.CultureInfo.InvariantCulture);
}

/// <summary>One natural local-seed Scots pine recruit (an individual, like a planted juvenile). Dead or promoted recruits stay listed.</summary>
internal sealed record ScotsPineRecruit(string RecruitId, string AreaId, float XM, float ZM, int EstablishYear, float AgeYears,
    float HeightM, bool Alive, string? PromotedTreeId, int DeathYear, string? DeathCause, int LastAssessmentYear)
{
    public bool IsActive => Alive && PromotedTreeId is null;
}

/// <summary>One Scots pine year: seed year, local recruitment, juvenile outcomes and adult growth-domain counts (diagnostics).</summary>
internal sealed record ScotsPineYearAccount(int YearBefore, int YearAfter, bool GoodSeedYear, int MatureSeedTrees, int EligibleCells,
    int Recruited, int EstablishmentDeaths, int JuvenileDeaths, int Promoted, int AdultsGrowing, int AdultsOutsideDomain);

/// <summary>
/// The Scots pine and site-moisture part of the one commit root (optional; null on every woodland that never chose a moisture
/// class or Scots pine, so their roots, tokens and saves are unchanged). Pine adults are named in the Phase 2 species map and
/// planted pine juveniles live in the Phase 2 juvenile list with species "scots-pine"; this record holds what Phase 2 does not.
/// </summary>
internal sealed record MultiAreaScotsPineState(
    SiteMoistureSetting Site,
    IReadOnlyList<string> TightEarlyStockingTreeIds,
    IReadOnlyList<ScotsPineRecruit> Recruits,
    int NextRecruitNumber,
    IReadOnlyList<ScotsPineYearAccount> Accounts)
{
    public static MultiAreaScotsPineState Create(SiteMoistureSetting site) => new(site.RequireValid(), Array.Empty<string>(),
        Array.Empty<ScotsPineRecruit>(), 1, Array.Empty<ScotsPineYearAccount>());

    public bool HasActiveRecruits => Recruits.Any(r => r.IsActive);

    public bool IsTight(string treeId) => TightEarlyStockingTreeIds.Contains(treeId, StringComparer.Ordinal);
}

/// <summary>Genesis inputs of a Scots pine / site-moisture woodland: the site layout and the trees that start as Scots pine.</summary>
internal sealed record ScotsPineGenesis(SiteMoistureSetting Site, IReadOnlyList<string> PineTreeIds);

/// <summary>Running adult growth-domain counts of one year (diagnostics only).</summary>
internal sealed class ScotsPineAdultTally
{
    public int Growing, OutsideDomain;
}

/// <summary>Per-rack context handed to the Phase 2 adult step when a woodland has Scots pine / site-moisture state.</summary>
internal sealed class ScotsPineRackContext
{
    private readonly Func<string, string> speciesOf;

    public ScotsPineRackContext(SiteMoistureField field, Func<string, string> speciesOf, ScotsPineAdultTally tally)
    {
        Field = field;
        this.speciesOf = speciesOf;
        Tally = tally;
    }

    public SiteMoistureField Field { get; }
    public ScotsPineAdultTally Tally { get; }

    public bool IsPine(string treeId) => ScotsPineLive.IsPine(speciesOf(treeId));

    /// <summary>The site multiplier of a live species at a tree: the Stage 4 site value times the moisture growth modifier.</summary>
    public float SiteFor(Stage4Options options, AnnualTree tree, string liveSpeciesId)
    {
        float site = options.SiteAt(tree);
        return Field.IsNeutral ? site : site * Field.GrowthMultiplier(liveSpeciesId, tree.XM, tree.ZM);
    }

    /// <summary>The unchanged options on a neutral site; otherwise the Sitka moisture growth modifier folded into the site field.</summary>
    public Stage4Options SitkaOptions(Stage4Options options)
    {
        if (Field.IsNeutral) return options;
        var baseField = options.SiteProductivity;
        var field = Field;
        return options with { SiteProductivity = (x, z) => baseField(x, z) * field.GrowthMultiplier(Phase2Species.SitkaId, x, z) };
    }
}

/// <summary>Scots pine adult growth (Part 1 GROWFOR-domain DBH increment) and CrownModel 1 crowns (Part 1 card crown fit).</summary>
internal static class ScotsPineAdult
{
    private static float Floor => (float)CrownProfile.MinimumRadiusM;

    /// <summary>[Calibration] C2 relaxation rate shared with the Sitka reference profile; the pine card authors none.</summary>
    public static float CrownRelaxation => SpeciesProfile.Sitka.CrownRelaxation;

    public static float Ci50 => (float)ScotsPineLive.Profile.Crown.Ci50Default;

    public static float PotentialRadiusM(float dbhCm) => (float)ScotsPineLive.Profile.Crown.ComputePotentialCrownRadiusM(dbhCm);

    /// <summary>r_target = max(0.1, r_pot(DBH) / sqrt(1 + CI/Ci50) · 0.92^lifts), the C2 target with the pine card's r_pot and Ci50.</summary>
    public static float TargetRadiusM(float dbhCm, float ci, int pruningLifts = 0) =>
        MathF.Max(Floor, PotentialRadiusM(dbhCm) / MathF.Sqrt(1f + ci / Ci50) * PruningRules.TargetFactor(pruningLifts));

    public static float NextRadiusM(float radiusM, float dbhCm, float ci, int pruningLifts) =>
        MathF.Max(Floor, radiusM + (TargetRadiusM(dbhCm, ci, pruningLifts) - radiusM) * CrownRelaxation);

    /// <summary>
    /// One year of a living Scots pine. DBH: the Part 1 site-adjusted increment for the tree's age and its rack's stocking,
    /// basal area and top height (no DBH growth outside the GROWFOR domain). Height: the shared Stage 4 height step scaled by
    /// the moisture growth modifier ([Calibration]; Part 1 supplies DBH only). Crown: the pine C2 relaxation step.
    /// </summary>
    public static AnnualTree Grow(AnnualTree tree, float woodlandCi, AnnualStand rackStand, double topHeightM, SiteMoistureField field,
        Stage4Options options, int pruningLifts, ScotsPineAdultTally? tally)
    {
        if (!tree.IsLiving) return tree;
        var growth = Evaluate(tree, rackStand, topHeightM, field);
        if (tally is not null)
        {
            if (growth.IsWithinCalibrationDomain) tally.Growing++;
            else tally.OutsideDomain++;
        }
        var shared = AnnualRules.GrowStage4(tree, woodlandCi, options.SiteAt(tree), options.GrowthAgePhases, pruningLifts);
        float heightGain = (shared.HeightM - tree.HeightM) * (float)growth.Site.Cell.Response.GrowthMultiplier;
        float dbh = Math.Clamp(tree.DbhCm + (float)(growth.SiteAdjustedDbhIncrementCm ?? 0.0), 1f, 200f);
        float height = Math.Clamp(tree.HeightM + heightGain, 0.1f, 60f);
        float radius = NextRadiusM(tree.CrownRadiusM, dbh, woodlandCi, pruningLifts);
        return tree with { AgeYears = checked(tree.AgeYears + 1), DbhCm = dbh, HeightM = height, CrownRadiusM = radius };
    }

    /// <summary>The Part 1 growth evaluation for one stem in its rack (also used by the inspector).</summary>
    public static ScotsPineGrowthResult Evaluate(AnnualTree tree, AnnualStand rackStand, double topHeightM, SiteMoistureField field) =>
        ScotsPineEngine.EvaluateGrowth(new ScotsPineGrowthRequest(tree.AgeYears, rackStand.StemsPerHa, rackStand.BasalAreaM2PerHa,
            topHeightM, field.At(tree.XM, tree.ZM)));

    /// <summary>A new pine adult (natural or planted): DBH = clamp(1.5·H, 2, 20) cm (the shared promotion rule), r_0 = pine C2 target.</summary>
    public static AnnualTree NewAdult(string treeId, float xM, float zM, int ageYears, float heightM, AnnualTree[] woodland)
    {
        float dbh = Math.Clamp(heightM * Stage5Constants.PromotionDbhCmPerMetreHeight, Stage5Constants.PromotionDbhMinCm,
            Stage5Constants.PromotionDbhMaxCm);
        var probe = new AnnualTree(treeId, xM, zM, ageYears, dbh, heightM, 0f);
        float ci = CrownModel.WoodlandCompetition(probe, woodland).Ci;
        return probe with { CrownRadiusM = TargetRadiusM(dbh, ci) };
    }
}

/// <summary>Scots pine pruning guards: two lifts (2.5 m then 5.5 m), five years apart, below 60 % of tree height.</summary>
internal static class ScotsPinePruning
{
    public static float NextLiftTargetM(int lifts) =>
        lifts >= 0 && lifts < ScotsPineLive.LiftTargetHeightsM.Count ? ScotsPineLive.LiftTargetHeightsM[lifts] : float.NaN;

    public static string? RefusalCode(AnnualTree tree, TreePruningState state, float target, int year)
    {
        if (!tree.IsLiving) return "PruneTreeNotLiving";
        if (state.Lifts >= ScotsPineLive.MaxLifts) return "PruneMaxLiftsReached";
        if (state.LastPruningYear >= 0 && year - state.LastPruningYear < PruningRules.MinimumYearsBetweenLifts) return "PruneIntervalTooShort";
        if (target <= state.CrownBaseHeightM) return "PruneTargetNotAboveCrownBase";
        if (float.IsNaN(target) || float.IsInfinity(target) || target <= 0f || target >= tree.HeightM * PruningRules.MaximumTargetHeightFraction)
            return "PruneTargetOutOfRange";
        if (target != NextLiftTargetM(state.Lifts)) return "PrunePineTargetInvalid";
        return null;
    }

    public static string Message(string code) => code switch
    {
        "PruneMaxLiftsReached" => "this Scots pine already has both pruning lifts (2.5 m and 5.5 m)",
        "PrunePineTargetInvalid" => "Scots pine pruning requires the next 2.5 m or 5.5 m lift (the 5–6 m clear-bole target)",
        _ => PruningRules.Message(code),
    };

    public static void Require(AnnualTree tree, TreePruningState state, float target, int year)
    {
        if (RefusalCode(tree, state, target, year) is string code) throw new P1Refusal(code, Message(code));
    }
}

/// <summary>The live inputs of a Scots pine grade, read from map state, handed straight to <see cref="ScotsPineEngine.GradeTimber"/>.</summary>
internal static class ScotsPineGrading
{
    public static ScotsPineTimberResult Grade(AnnualTree tree, decimal volumeM3, TreePruningState pruning, bool tightEarlyStocking) =>
        ScotsPineEngine.GradeTimber(new ScotsPineTimberRequest(
            volumeM3,
            IsMature: tree.AgeYears >= ScotsPineLive.MatureAgeYears,
            PrunedHeightM: pruning.Lifts > 0 ? pruning.CrownBaseHeightM : 0.0,
            HadTightEarlyStocking: tightEarlyStocking,
            SuitableForSmallRoundwood: tree.DbhCm >= ScotsPineLive.SmallRoundwoodMinimumDbhCm));
}

/// <summary>The outcome of one Scots pine year.</summary>
internal sealed record ScotsPineAnnualResult(MultiAreaPhase2State NextPhase2, MultiAreaScotsPineState NextState,
    IReadOnlyDictionary<string, IReadOnlyList<AnnualTree>> PromotedTrees, ScotsPineYearAccount Account);

/// <summary>
/// The Scots pine regeneration year, run after the Phase 2 step on its next state: planted pine juveniles and natural recruits
/// grow, die or promote on this year's cell light and covers, then local-seed recruitment fills eligible gap cells.
/// </summary>
internal static class ScotsPineAnnual
{
    public static ScotsPineAnnualResult Step(MultiAreaScotsPineState start, MultiAreaPhase2State phase2, MultiAreaStage5State stage5,
        IReadOnlyList<Stage5AreaInput> areas, SiteMoistureField field, ulong woodlandSeed, int yearAfter, ScotsPineAdultTally adults)
    {
        int year = checked(yearAfter - 1);
        float pressure = stage5.Options.BackgroundBrowsePressure;
        var locate = Phase2Annual.Locator(stage5);
        var byArea = areas.ToDictionary(area => area.AreaId, StringComparer.Ordinal);
        var woodland = CrownModel.Woodland(areas.SelectMany(area => area.Trees));
        var existingIds = areas.SelectMany(area => area.Trees).Select(tree => tree.TreeId).ToHashSet(StringComparer.Ordinal);
        var speciesMap = new Dictionary<string, string>(phase2.TreeSpecies, StringComparer.Ordinal);
        var promoted = areas.ToDictionary(area => area.AreaId, _ => new List<AnnualTree>(), StringComparer.Ordinal);
        var tight = start.TightEarlyStockingTreeIds.ToList();
        int establishmentDeaths = 0, juvenileDeaths = 0, promotedCount = 0;

        (Stage5Cell Cell, int Area, int Index)? CellAt(float x, float z) =>
            locate(x, z) is var (a, c) ? (stage5.Areas[a].Cells[c], a, c) : null;

        void Promote(string treeId, string areaId, float x, float z, float age, float height)
        {
            if (!existingIds.Add(treeId)) throw new P1Refusal("Stage5TreeIdNotUnique", "a promoted tree ID already exists: " + treeId);
            var tree = ScotsPineAdult.NewAdult(treeId, x, z, Math.Max(1, (int)Math.Round(age, MidpointRounding.ToEven)), height, woodland);
            promoted[areaId].Add(tree);
            speciesMap[treeId] = ScotsPineLive.SpeciesId;
            var input = byArea[areaId];
            double stemsPerHa = input.Trees.Count(t => t.IsLiving) / (double)input.AreaHa;
            if (stemsPerHa >= ScotsPineLive.TightEarlyStockingStemsPerHa) tight.Add(treeId);
            promotedCount++;
        }

        // 1. Planted Scots pine juveniles (Phase 2 list, species scots-pine), ordinal juvenile ID order.
        var juveniles = phase2.Juveniles.ToDictionary(j => j.JuvenileId, StringComparer.Ordinal);
        foreach (var juvenile in phase2.Juveniles.Where(j => j.IsActive && ScotsPineLive.IsPine(j.SpeciesId))
                     .OrderBy(j => j.JuvenileId, StringComparer.Ordinal))
        {
            if (CellAt(juvenile.XM, juvenile.ZM) is not var (cell, _, _)) continue;
            int seed = byArea[juvenile.AreaId].SimulationSeed;
            float age = juvenile.AgeYears + 1f;
            // First assessed year only: planting establishment on a site whose modifier is below 1 (no draw on a neutral site).
            float establishment = field.EstablishmentMultiplier(ScotsPineLive.SpeciesId, juvenile.XM, juvenile.ZM);
            if (juvenile.LastAssessmentYear < 0 && establishment < 1f &&
                Phase2Random.Roll(RngSalts.ScotsPineEstablishment + juvenile.JuvenileId, yearAfter, seed)
                    >= establishment)
            {
                juveniles[juvenile.JuvenileId] = juvenile with
                {
                    AgeYears = age, Alive = false, DeathYear = yearAfter, DeathCause = "establishment", LastAssessmentYear = yearAfter,
                };
                establishmentDeaths++;
                continue;
            }
            bool sheltered = Phase2Annual.AccessAt(phase2.Shelters, juvenile.XM, juvenile.ZM, yearAfter) == 0f;
            float exposure = Phase2Annual.ExposureAt(phase2.Patches, juvenile.XM, juvenile.ZM, (cell.Bramble, cell.Bracken));
            var outcome = Juvenile(juvenile.HeightM, cell.Light, exposure, sheltered, pressure,
                field.GrowthMultiplier(ScotsPineLive.SpeciesId, juvenile.XM, juvenile.ZM));
            var updated = juvenile with
            {
                AgeYears = age, HeightM = outcome.Height, LastYearBrowsed = outcome.Browsed,
                LastBrowseProbability = outcome.Browsed ? pressure : 0f, LastVegetationLossProbability = exposure,
                LastAssessmentYear = yearAfter,
            };
            if (outcome.DeathCause is string cause)
            {
                juveniles[juvenile.JuvenileId] = updated with { Alive = false, DeathYear = yearAfter, DeathCause = cause };
                juvenileDeaths++;
                continue;
            }
            if (outcome.Height >= ScotsPineLive.PromotionHeightM)
            {
                string treeId = "PL-" + juvenile.JuvenileId;
                Promote(treeId, juvenile.AreaId, juvenile.XM, juvenile.ZM, age, outcome.Height);
                updated = updated with { PromotedTreeId = treeId };
            }
            juveniles[juvenile.JuvenileId] = updated;
        }

        // 2. Natural recruits, ordinal recruit ID order (they are never browsed under a shelter: none exist for them).
        var recruits = start.Recruits.ToList();
        for (int r = 0; r < recruits.Count; r++)
        {
            var recruit = recruits[r];
            if (!recruit.IsActive || CellAt(recruit.XM, recruit.ZM) is not var (cell, _, _)) continue;
            float age = recruit.AgeYears + 1f;
            var outcome = Juvenile(recruit.HeightM, cell.Light, MathF.Max(cell.Bramble, cell.Bracken), sheltered: false, pressure,
                field.GrowthMultiplier(ScotsPineLive.SpeciesId, recruit.XM, recruit.ZM));
            var updated = recruit with { AgeYears = age, HeightM = outcome.Height, LastAssessmentYear = yearAfter };
            if (outcome.DeathCause is string cause)
            {
                recruits[r] = updated with { Alive = false, DeathYear = yearAfter, DeathCause = cause };
                juvenileDeaths++;
                continue;
            }
            if (outcome.Height >= ScotsPineLive.PromotionHeightM)
            {
                string treeId = "PN-" + recruit.RecruitId;
                Promote(treeId, recruit.AreaId, recruit.XM, recruit.ZM, age, outcome.Height);
                updated = updated with { PromotedTreeId = treeId };
            }
            recruits[r] = updated;
        }

        // 3. Local-seed recruitment. Nothing is drawn without a mature on-map pine or in a poor seed year.
        bool goodSeedYear = ScotsPineEngine.IsGoodSeedYear(woodlandSeed, year);
        var seedTrees = woodland.Where(tree => ScotsPineLive.IsPine(speciesMap.GetValueOrDefault(tree.TreeId)) &&
            tree.AgeYears >= ScotsPineEngine.MinimumSeedTreeAgeYears).ToArray();
        int eligibleCells = 0, recruited = 0, nextRecruit = start.NextRecruitNumber;
        if (goodSeedYear && seedTrees.Length > 0)
        {
            var occupied = new HashSet<(int, int)>();
            foreach (var j in juveniles.Values.Where(j => j.IsActive && ScotsPineLive.IsPine(j.SpeciesId)))
                if (locate(j.XM, j.ZM) is var (a, c)) occupied.Add((a, c));
            foreach (var rec in recruits.Where(rec => rec.IsActive))
                if (locate(rec.XM, rec.ZM) is var (a, c)) occupied.Add((a, c));
            double radiusSq = ScotsPineLive.SeedTreeSearchRadiusM * ScotsPineLive.SeedTreeSearchRadiusM;
            for (int a = 0; a < stage5.Areas.Count; a++)
            {
                var input = byArea[stage5.Areas[a].AreaId];
                var cells = stage5.Areas[a].Cells;
                for (int c = 0; c < cells.Count; c++)
                {
                    var cell = cells[c];
                    if (occupied.Contains((a, c)) || !Stage5Promotion.StrictlyInside(input.Polygon, cell.CentreXM, cell.CentreZM)) continue;
                    int? oldest = null;
                    foreach (var tree in seedTrees)
                    {
                        double dx = tree.XM - cell.CentreXM, dz = tree.ZM - cell.CentreZM;
                        if (dx * dx + dz * dz <= radiusSq && (oldest is null || tree.AgeYears > oldest)) oldest = tree.AgeYears;
                    }
                    if (oldest is null) continue;
                    var gate = ScotsPineEngine.EvaluateRegeneration(new ScotsPineRegenerationRequest(woodlandSeed, year, oldest,
                        Math.Clamp((double)cell.Light, 0.0, 1.0), Math.Clamp((double)MathF.Max(cell.Bramble, cell.Bracken), 0.0, 1.0),
                        field.At(cell.CentreXM, cell.CentreZM)));
                    if (!gate.CanRegenerate) continue;
                    eligibleCells++;
                    double chance = ScotsPineLive.BaseRecruitProbability * gate.EstablishmentMultiplier;
                    string key = RngSalts.ScotsPineRecruitment + input.AreaId + "-" +
                        cell.Column.ToString(System.Globalization.CultureInfo.InvariantCulture) + "-" +
                        cell.Row.ToString(System.Globalization.CultureInfo.InvariantCulture);
                    if (Phase2Random.Roll(key, yearAfter, input.SimulationSeed) >= chance) continue;
                    recruits.Add(new ScotsPineRecruit(ScotsPineLive.RecruitIdFor(nextRecruit++), input.AreaId, cell.CentreXM, cell.CentreZM,
                        yearAfter, 1f, ScotsPineLive.RecruitInitialHeightM, true, null, -1, null, yearAfter));
                    recruited++;
                }
            }
        }

        var account = new ScotsPineYearAccount(year, yearAfter, goodSeedYear, seedTrees.Length, eligibleCells, recruited,
            establishmentDeaths, juvenileDeaths, promotedCount, adults.Growing, adults.OutsideDomain);
        var nextPhase2 = phase2 with
        {
            TreeSpecies = speciesMap,
            Juveniles = Array.AsReadOnly(phase2.Juveniles.Select(j => juveniles[j.JuvenileId]).ToArray()),
        };
        var nextState = start with
        {
            TightEarlyStockingTreeIds = Array.AsReadOnly(tight.Distinct(StringComparer.Ordinal).Order(StringComparer.Ordinal).ToArray()),
            Recruits = recruits.AsReadOnly(),
            NextRecruitNumber = nextRecruit,
            Accounts = Array.AsReadOnly(start.Accounts.Append(account).ToArray()),
        };
        return new ScotsPineAnnualResult(nextPhase2, nextState,
            promoted.ToDictionary(pair => pair.Key, pair => (IReadOnlyList<AnnualTree>)pair.Value.OrderBy(t => t.TreeId, StringComparer.Ordinal)
                .ToArray(), StringComparer.Ordinal), account);
    }

    /// <summary>
    /// One deterministic juvenile year ([Calibration]): death in a cell darker than 0.20 or when smothered below 1.0 m; otherwise
    /// growth = 0.40 m · min(1, light / 0.60) · moisture growth modifier, reduced by the browse pressure when unsheltered below 1.5 m.
    /// </summary>
    internal static (float Height, bool Browsed, string? DeathCause) Juvenile(float height, float light, float exposure, bool sheltered,
        float pressure, float growthMultiplier)
    {
        if (light < ScotsPineLive.JuvenileMinimumLight) return (height, false, "light");
        if (height < ScotsPineLive.SmotheringHeightM && exposure >= ScotsPineLive.SmotheringExposure) return (height, false, "vegetation");
        bool browsed = !sheltered && pressure > 0f && height < ScotsPineLive.BrowseEscapeHeightM;
        float lightFactor = MathF.Min(1f, light / (float)ScotsPineEngine.MinimumCanopyOpenness);
        float gain = ScotsPineLive.JuvenileHeightGrowthMPerYear * lightFactor * growthMultiplier * (browsed ? 1f - pressure : 1f);
        return (height + MathF.Max(0f, gain), browsed, null);
    }
}
