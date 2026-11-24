using System.Globalization;
using SitkaP1.Core;
using static SitkaP1.Core.Tests.Stage5Support;

namespace SitkaP1.Core.Tests;

/// <summary>Shared helpers for the MULTI Phase 2 qualification tests: fixtures, sessions, pure-step states and fingerprints.</summary>
internal static class Phase2Support
{
    internal const string Oak = Phase2Species.OakId;
    internal const string Beech = Phase2Species.BeechId;
    internal const string Sitka = Phase2Species.SitkaId;

    internal static MultiAreaContinuation Phase2Session(MultiAreaExperiment? experiment = null, Stage5Options? stage5 = null,
        Phase2Options? phase2 = null) =>
        new(experiment ?? MultiAreaGenerator.Generate(), MultiAreaModelSelection.Phase2, null, stage5, phase2);

    /// <summary>
    /// MA-1-MIXED-v1 (test-only, owner Q5). Derived from MA-1 without touching the generator, /multi or any default:
    /// Area A stays 100 % Sitka. Area B's stems, in ordinal TreeId order with index i from 0, are reassigned by i mod 3:
    /// 0 → sessile-oak, 1 → beech, 2 → stays sitka-spruce. Each reassigned stem keeps its position, DBH and height and has
    /// its age set to 60; the Sitka third keeps its MA-1 age. The rule is fixed here before any qualification number is read.
    /// </summary>
    internal static (MultiAreaExperiment Experiment, Phase2Options Options) MixedFixture(Phase2Options? baseOptions = null)
    {
        var ma1 = MultiAreaGenerator.Generate();
        var species = new Dictionary<string, string>(StringComparer.Ordinal);
        var areas = ma1.Areas.Select(area =>
        {
            if (area.AreaId != "MA-B") return area;
            var trees = area.Trees.OrderBy(t => t.TreeId, StringComparer.Ordinal).Select((t, i) =>
            {
                string? id = (i % 3) switch { 0 => Oak, 1 => Beech, _ => null };
                if (id is null) return t;
                species[t.TreeId] = id;
                return t with { AgeYears = 60 };
            }).ToArray();
            return area with { Trees = Array.AsReadOnly(trees) };
        }).ToArray();
        var experiment = new MultiAreaExperiment("mad-ma1-mixed-v1", ma1.GeneratorId, ma1.GeneratorVersion, ma1.GeneratorSeed,
            Array.AsReadOnly(areas));
        experiment.Validate();
        return (experiment, (baseOptions ?? Phase2Options.Default) with { GenesisSpecies = species });
    }

    internal static MultiAreaContinuation MixedSession(Stage5Options? stage5 = null, Phase2Options? baseOptions = null)
    {
        var (experiment, options) = MixedFixture(baseOptions);
        return Phase2Session(experiment, stage5, options);
    }

    /// <summary>
    /// Two rectangular racks side by side (A west of B), each width × height metres. Trees are (x mm, y mm, age, DBH mm,
    /// height mm, species); every rack needs at least one stem.
    /// </summary>
    internal static (MultiAreaExperiment Experiment, Phase2Options Options) TwoRacks(string id, int widthM, int heightM,
        (int X, int Y, int Age, int DbhMm, int HeightMm, string Species)[] a, (int X, int Y, int Age, int DbhMm, int HeightMm, string Species)[] b)
    {
        static MillimetrePoint P(long x, long y) => new(x, y);
        var species = new Dictionary<string, string>(StringComparer.Ordinal);
        MultiAreaTreeDefinition[] Trees(string areaId, (int X, int Y, int Age, int DbhMm, int HeightMm, string Species)[] list) =>
            list.Select((t, i) =>
            {
                string treeId = areaId + "-" + (i + 1).ToString("D3", CultureInfo.InvariantCulture);
                if (t.Species != Sitka) species[treeId] = t.Species;
                return new MultiAreaTreeDefinition(treeId, t.X, t.Y, t.Age, t.DbhMm, t.HeightMm);
            }).ToArray();
        long w = widthM * 1000L, h = heightM * 1000L;
        var experiment = new MultiAreaExperiment(id, "p2-test", 1, 9, new[]
        {
            new MultiAreaDefinition("MA-A", "Area A", "P2 test", new[] { P(0, 0), P(w, 0), P(w, h), P(0, h) }, 901, Trees("MA-A", a)),
            new MultiAreaDefinition("MA-B", "Area B", "P2 test", new[] { P(w, 0), P(2 * w, 0), P(2 * w, h), P(w, h) }, 902, Trees("MA-B", b)),
        });
        return (experiment, Phase2Options.Default with { GenesisSpecies = species });
    }

    internal static MultiAreaPhase2State Phase2Of(MultiAreaContinuation c) => Root(c).Phase2!;
    internal static MultiAreaStage5State Stage5Of(MultiAreaContinuation c) => Root(c).Stage5!;

    /// <summary>Approve and execute whatever is pending; returns the execution result.</summary>
    internal static MultiAreaCommandResult ApproveAndExecute(MultiAreaContinuation c)
    {
        var v = c.Approve(Fence(c.ReadView())).View;
        return c.Execute(Fence(v), v.Approved!.PlanId, v.Approved.ExecutionKey);
    }

    /// <summary>
    /// The first point (row-major 0.25 m scan of the area's bounding box) strictly inside the area, inside an in-polygon
    /// ground cell, at least <paramref name="minAdultM"/> from every living adult and accepted by <paramref name="accept"/>.
    /// </summary>
    internal static (float X, float Z) FindPoint(MultiAreaContinuation c, string areaId, float minAdultM, Func<float, float, bool>? accept = null)
    {
        var v = c.ReadView();
        var area = v.Areas.Single(a => a.AreaId == areaId);
        var adults = v.Areas.SelectMany(a => a.Trees).Where(t => t.IsLiving).ToArray();
        var locate = Phase2Annual.Locator(Stage5Of(c));
        double minX = area.Polygon.Min(p => p.XM), maxX = area.Polygon.Max(p => p.XM);
        double minZ = area.Polygon.Min(p => p.ZM), maxZ = area.Polygon.Max(p => p.ZM);
        for (double z = Math.Ceiling(minZ) + 0.125; z < maxZ; z += 0.25)
            for (double x = Math.Ceiling(minX) + 0.125; x < maxX; x += 0.25)
            {
                float fx = (float)x, fz = (float)z;
                if (!Stage5Promotion.StrictlyInside(area.Polygon, fx, fz) || locate(fx, fz) is null) continue;
                if (adults.Any(t => (t.XM - fx) * (t.XM - fx) + (t.ZM - fz) * (t.ZM - fz) < minAdultM * minAdultM)) continue;
                if (accept is not null && !accept(fx, fz)) continue;
                return (fx, fz);
            }
        throw new Exception("no planting point found in " + areaId);
    }

    // ---------------------------------------------------------------- pure-step helpers

    /// <summary>The Stage 5 and Phase 2 genesis of two empty 10 × 10 m racks (light 1 everywhere) with a species map.</summary>
    internal static (MultiAreaStage5State S5, MultiAreaPhase2State S2) EmptyStates(Stage5AreaInput[] inputs, Stage5Options? options = null,
        IReadOnlyDictionary<string, string>? species = null, int mastSeed = 5)
    {
        var s5 = Stage5Annual.Genesis(options ?? Stage5Options.Default, mastSeed, inputs);
        var s2 = Phase2Annual.Genesis(Phase2Options.Default, mastSeed, s5, species ?? new Dictionary<string, string>(StringComparer.Ordinal));
        return (s5, s2);
    }

    /// <summary>The model light field of the inputs with every cell's light index replaced by <paramref name="light"/>.</summary>
    internal static IReadOnlyDictionary<string, IReadOnlyList<CanopyLightCell>> FixedLight(Stage5AreaInput[] inputs, double gridM,
        Func<string, CanopyLightCell, float> light)
    {
        var areas = inputs.Select(area => new CanopyArea(area.AreaId,
            Array.AsReadOnly(area.Polygon.Select(v => new CanopyVertex(v.XM, v.ZM)).ToArray()), area.AreaHa, area.Trees)).ToArray();
        var fields = CanopyLightEvaluator.LightFields(areas, gridM);
        return inputs.Select((area, i) => (area.AreaId, (IReadOnlyList<CanopyLightCell>)Array.AsReadOnly(
                fields[i].Cells.Select(cell => cell with { LightIndex = light(area.AreaId, cell) }).ToArray())))
            .ToDictionary(pair => pair.AreaId, pair => pair.Item2, StringComparer.Ordinal);
    }

    internal static int CellIndex(MultiAreaStage5State s5, string areaId, int column, int row)
    {
        var cells = s5.Areas.Single(a => a.AreaId == areaId).Cells;
        for (int i = 0; i < cells.Count; i++)
            if (cells[i].Column == column && cells[i].Row == row) return i;
        throw new Exception("no cell " + column + "," + row);
    }

    internal static MultiAreaPhase2State WithBroadleafBands(MultiAreaPhase2State s, string areaId, int index, params Phase2Band[] bands) =>
        s with
        {
            Areas = Array.AsReadOnly(s.Areas.Select(a => a.AreaId != areaId ? a : a with
            {
                Cells = Array.AsReadOnly(a.Cells.Select((cell, i) => i == index ? cell with { Bands = Array.AsReadOnly(bands) } : cell).ToArray()),
            }).ToArray()),
        };

    internal static Phase2Band BroadleafBand(string species, float density, float height, int year) =>
        new(species, Phase2Band.Natural, density, height, year, year, 0f, -1);

    internal static PlantedJuvenile Juvenile(string id, string areaId, string species, float x, float z, float age, float height) =>
        new(id, areaId, species, x, z, 0, age, height, true, null, -1, null, false, 0f, 0f, -1);

    /// <summary>Shared relative occupancy of one cell over every species (Sitka bands in Stage 5, broadleaves in Phase 2).</summary>
    internal static double CellOccupancy(Stage5Cell s5Cell, Phase2CellBands broadleaf, double capacity) =>
        (s5Cell.Bands.Where(b => b.Density > 0f).Sum(b => (double)b.Density) +
         broadleaf.Bands.Where(b => b.Density > 0f).Sum(b => (double)b.Density)) / capacity;

    // ---------------------------------------------------------------- fingerprints

    private static string F(float v) => F32.Hex(F32.Bits(v));
    private static string D(double v) => BitConverter.DoubleToUInt64Bits(v).ToString("x16", CultureInfo.InvariantCulture);

    /// <summary>Every field of a Phase 2 state, floats as IEEE-754 bits.</summary>
    internal static void WritePhase2(Model0FingerprintTests.Fingerprint w, MultiAreaPhase2State s)
    {
        w.Text("phase2 seeds " + s.OakMastSeed + "|" + s.BeechMastSeed + "|" + s.NextOrderNumber + "|" + s.NextJuvenileNumber + "|" + s.NextPatchNumber);
        foreach (var (tree, species) in s.TreeSpecies.OrderBy(p => p.Key, StringComparer.Ordinal)) w.Text("species|" + tree + "|" + species);
        foreach (var area in s.Areas)
        {
            w.Text(area.AreaId + " cells " + area.Cells.Count);
            foreach (var cell in area.Cells)
                foreach (var b in cell.Bands)
                    w.Text(string.Join("|", cell.Column, cell.Row, b.SpeciesId, b.Origin, F(b.Density), F(b.Height), b.EstablishYear, b.OriginYear,
                        F(b.LastBrowsedFraction), b.LastBrowseAssessmentYear));
        }
        foreach (var j in s.Juveniles)
            w.Text(string.Join("|", "pj", j.JuvenileId, j.AreaId, j.SpeciesId, F(j.XM), F(j.ZM), j.PlantingYear, F(j.AgeYears), F(j.HeightM), j.Alive,
                j.PromotedTreeId ?? "-", j.DeathYear, j.DeathCause ?? "-", j.LastYearBrowsed, F(j.LastBrowseProbability),
                F(j.LastVegetationLossProbability), j.LastAssessmentYear));
        foreach (var sh in s.Shelters)
            w.Text(string.Join("|", "shelter", sh.ShelterId, sh.JuvenileId, F(sh.XM), F(sh.ZM), sh.InstalledYear, sh.EffectiveYears, sh.FailedYear));
        foreach (var p in s.Patches)
            w.Text(string.Join("|", "patch", p.PatchId, p.AreaId, F(p.XM), F(p.ZM), F(p.RadiusM), p.CreatedYear, p.UpdatedYear, F(p.Bramble), F(p.Bracken)));
        foreach (var o in s.PendingOrders.Concat(s.ApprovedOrders))
            w.Text(string.Join("|", "order", o.GetType().Name, o.OrderId, o.AreaId, F(o.XM), F(o.ZM), (o as MultiAreaPlantOrder)?.SpeciesId ?? "-"));
        foreach (var r in s.WorkLog)
            w.Text(string.Join("|", "work", r.OrderId, r.Kind, r.AreaId, F(r.XM), F(r.ZM), r.SpeciesId ?? "-", r.JuvenileId ?? "-", r.ShelterId ?? "-",
                r.PatchId ?? "-", r.EcologicalYear, r.WorkRevision));
        foreach (var y in s.Accounts)
        {
            w.Text("p2year|" + y.YearBefore + "|" + y.YearAfter);
            foreach (var m in y.Mast) w.Text(string.Join("|", "mast", m.SpeciesId, m.MastSeed, D(m.Roll), m.Label, F(m.Multiplier)));
            foreach (var a in y.Natural)
            {
                w.Text(string.Join("|", "natural", a.AreaId, a.SpeciesId, D(a.SeedArrival), D(a.StartingAbundance), D(a.VegetationLoss), D(a.LightLoss),
                    D(a.BrowseLoss), D(a.EstablishmentRequested), D(a.EstablishmentAccepted), D(a.CapacityRejected), a.BandsCreated, a.BandMerges,
                    a.ThresholdCrossings, a.QualifyingCells, a.BlockCapDeferredCells, D(a.PromotionExported), D(a.EndingAbundance),
                    a.CohortBandsExported, a.BudgetWithheldBlocks));
                foreach (var p in a.Promotions)
                    w.Text(string.Join("|", "promo", p.TreeId, p.Column, p.Row, p.BlockX, p.BlockZ, p.EstablishYear, p.AgeYears, F(p.HeightM), F(p.DbhCm),
                        F(p.CrownRadiusM), F(p.WoodlandCi), F(p.XM), F(p.ZM), p.CentreFallback, F(p.ExportedDensity), p.CohortBandsExported));
            }
            foreach (var p in y.Planted)
                w.Text(string.Join("|", "planted", p.SpeciesId, p.Starting, p.VegetationDeaths, p.LightDeaths, p.BrowseDeaths, p.Remaining, p.Promoted,
                    string.Join(",", p.PromotedTreeIds)));
            foreach (var d in y.Density)
                w.Text(string.Join("|", "density", d.AreaId, d.Mixed, d.Stems, d.SitkaSubset, d.SitkaHazardDeaths, d.SitkaBoundaryDeaths, D(d.Occupancy),
                    D(d.GenericResponse), D(d.NonReferenceShare), d.EligibleForGeneric, D(d.ExpectedGeneric), d.GenericQuota, D(d.QuotaRemainder),
                    d.QuotaRoll is double q ? D(q) : "null", d.GenericDeaths, d.BudgetPassedOver, d.SingleStemFallback, D(d.StandBasalAreaM2),
                    D(d.GenericBasalAreaM2), string.Join(",", d.GenericVictims)));
        }
    }

    /// <summary>Deadwood and per-tree Stage 4 states of a session.</summary>
    internal static void WriteStage4(Model0FingerprintTests.Fingerprint w, MultiAreaStage4View s4)
    {
        foreach (var area in s4.Areas)
        {
            w.Text(area.AreaId + "|" + D(area.RelativeDensity) + "|" + area.AnnualNaturalDeaths + "|" + area.CumulativeNaturalDeaths);
            foreach (var d in area.Deadwood)
                w.Text(string.Join("|", d.DeadwoodId, d.TreeId, d.AreaId, d.SpeciesId, F(d.XM), F(d.ZM), d.DeathYear, d.Cause, F(d.DbhCm), F(d.HeightM),
                    F(d.EstimatedVolumeM3)));
            foreach (var t in area.Trees)
                w.Text(string.Join("|", t.TreeId, t.LifeState, t.DeathCause ?? "-", t.DeathYear?.ToString(CultureInfo.InvariantCulture) ?? "-",
                    t.PruningLifts, F(t.CrownBaseHeightM), t.LastPruningYear, F(t.CrownRadiusM)));
        }
        foreach (var p in s4.PruneLog)
            w.Text(string.Join("|", p.AreaId, p.TreeId, F(p.TargetCrownBaseHeightM), p.EcologicalYear, F(p.CrownRadiusBeforeM), F(p.CrownRadiusAfterM),
                p.LiftsAfter));
    }
}
