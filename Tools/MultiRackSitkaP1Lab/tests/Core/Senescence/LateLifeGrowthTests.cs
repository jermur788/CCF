using System.Globalization;
using SitkaP1.Core.NaturalProcesses.Senescence;
using SitkaP1.Core.SpeciesWave2;
using SitkaP1.Core.SpeciesWave2.BirchAlder;
using SitkaP1.Core.SpeciesWave2.ScotsPine;
using static SitkaP1.Core.Senescence.Tests.Fixtures;
using static SitkaP1.Core.Senescence.Tests.TestRunner;

namespace SitkaP1.Core.Senescence.Tests;

/// <summary>FR-2 / AC-4: Wave 2 late-life DBH growth continuation.</summary>
internal static class LateLifeGrowthTests
{
    private const string Suite = "LateLifeGrowth";

    private static readonly string[] Wave2 = { Pine, Alder, Birch };

    /// <summary>The species' existing growth domain, read from the Wave 2 sources (never restated here).</summary>
    private readonly record struct Domain(int Low, int Cutoff, ValueBand Stems, ValueBand Basal, double MinTop, double MaxReduction);

    private static Domain DomainOf(string species)
    {
        if (species == Pine)
        {
            ReadinessProfile r = FutureSpeciesCatalog.ScotsPine.Readiness;
            ValueBand ages = r.AgeRangeYears!.Value;
            // 0.50 is the FR-2 Scots pine maximum crowding reduction; ScotsPineGrowth.cs keeps it private.
            return new Domain((int)ages.Low, (int)ages.High, r.StemsPerHectare!.Value, r.BasalAreaM2PerHa!.Value,
                r.MinimumTopHeightM!.Value, 0.50);
        }
        GrowthTeachingDomain d = BirchAlderEngine.GrowthDomain(species);
        double reduction = species == Birch
            ? BirchAlderEngine.BirchMaximumCrowdingReduction
            : BirchAlderEngine.AlderMaximumCrowdingReduction;
        return new Domain((int)d.AgeYears.Low, (int)d.AgeYears.High, d.StemsPerHectare, d.BasalAreaM2PerHa, d.MinimumTopHeightM, reduction);
    }

    private static double At(ValueBand band, int step, int steps) => band.Low + (band.High - band.Low) * step / steps;

    /// <summary>What the existing Wave 2 evaluator returns: the reference LateLifeGrowth must reproduce in-window.</summary>
    private static (double? Potential, double? SiteAdjusted, SiteResponse Response) Delegated(
        string species, int age, double stems, double basal, double top, SiteMoistureClass moisture)
    {
        if (species == Pine)
        {
            ScotsPineGrowthResult p = ScotsPineEngine.EvaluateGrowth(new ScotsPineGrowthRequest(age, stems, basal, top, moisture));
            return (p.PotentialDbhIncrementCm, p.SiteAdjustedDbhIncrementCm, p.Site.Cell.Response);
        }
        BirchAlderGrowthResult b = BirchAlderEngine.EvaluateGrowth(new BirchAlderGrowthRequest(species, age, stems, basal, top, moisture));
        return (b.PotentialDbhIncrementCm, b.SiteAdjustedDbhIncrementCm, b.Site.Response);
    }

    private static LateLifeGrowthResult Late(string species, int age, double stems, double basal, double top, SiteMoistureClass moisture) =>
        LateLifeGrowth.EvaluateGrowth(new LateLifeGrowthRequest(species, age, stems, basal, top, moisture));

    private static double Nominal(string species, int age) => LateLifeGrowth.NominalIncrementAtOrBeyondCutoff(species, age);

    /// <summary>Zero-crowding NeutralLegacy potential from the existing evaluator.</summary>
    private static double DelegatedNominal(string species, int age)
    {
        Domain d = DomainOf(species);
        return Val(Delegated(species, age, d.Stems.Low, d.Basal.Low, d.MinTop, SiteMoistureClass.NeutralLegacy).Potential,
            $"{species} delegated nominal at {age}");
    }

    /// <summary>Bit-exact agreement with the existing evaluator; returns whether an increment was available.</summary>
    private static bool AssertDelegated(string species, int age, double stems, double basal, double top, SiteMoistureClass moisture)
    {
        string label = string.Create(CultureInfo.InvariantCulture, $"{species} age {age} stems {stems} BA {basal} top {top} {moisture}");
        var expected = Delegated(species, age, stems, basal, top, moisture);
        LateLifeGrowthResult actual = Late(species, age, stems, basal, top, moisture);
        SameBits(expected.Potential, actual.PotentialDbhIncrementCm, label + " potential");
        SameBits(expected.SiteAdjusted, actual.SiteAdjustedDbhIncrementCm, label + " site-adjusted");
        BitEq(expected.Response.EstablishmentMultiplier, actual.SiteResponse.EstablishmentMultiplier, label + " establishment multiplier");
        BitEq(expected.Response.GrowthMultiplier, actual.SiteResponse.GrowthMultiplier, label + " growth multiplier");
        Eq(expected.Potential.HasValue, actual.IsWithinCalibrationWindow, label + " window flag");
        Eq(expected.Potential.HasValue, actual.IsAvailable, label + " availability");
        Eq(species, actual.SpeciesId, label + " species");
        Eq(moisture, actual.Moisture, label + " moisture");
        return actual.IsAvailable;
    }

    /// <summary>GROW-01..03: one in-window age over a crowding x top-height x moisture grid.</summary>
    private static void AssertDelegatedAtAge(string species, int age, double tallTop)
    {
        Domain d = DomainOf(species);
        var potentials = new HashSet<long>();
        var siteAdjusted = new HashSet<long>();
        for (int i = 0; i <= 4; i++)
        for (int j = 0; j <= 4; j++)
        foreach (double top in new[] { d.MinTop, tallTop })
        foreach (SiteMoistureClass moisture in SpeciesMoistureTable.MoistureClasses)
        {
            double stems = At(d.Stems, i, 4);
            double basal = At(d.Basal, j, 4);
            True(AssertDelegated(species, age, stems, basal, top, moisture), $"{species} age {age} is inside the window");
            LateLifeGrowthResult r = Late(species, age, stems, basal, top, moisture);
            potentials.Add(BitConverter.DoubleToInt64Bits(Val(r.PotentialDbhIncrementCm, "potential")));
            siteAdjusted.Add(BitConverter.DoubleToInt64Bits(Val(r.SiteAdjustedDbhIncrementCm, "site-adjusted")));
        }
        True(potentials.Count >= 5, species + " crowding really varied the potential (five crowding levels)");
        True(siteAdjusted.Count > potentials.Count, species + " moisture really varied the site-adjusted increment");
    }

    public static void Register()
    {
        TestRunner.Add(Suite, "GROW-01 scots-pine age 42 delegates bit-exactly to ScotsPineEngine across crowding and moisture", () =>
            AssertDelegatedAtAge(Pine, 42, 18.5));

        TestRunner.Add(Suite, "GROW-02 common-alder age 33 delegates bit-exactly to BirchAlderEngine across crowding and moisture", () =>
            AssertDelegatedAtAge(Alder, 33, 14.0));

        TestRunner.Add(Suite, "GROW-03 downy-birch age 44 delegates bit-exactly to BirchAlderEngine across crowding and moisture", () =>
            AssertDelegatedAtAge(Birch, 44, 16.0));

        TestRunner.Add(Suite, "GROW-04 every age from 0 to the cutoff, in and out of the stand domain, matches the existing evaluator (nulls included)", () =>
        {
            foreach (string species in Wave2)
            {
                Domain d = DomainOf(species);
                double[] stems = { d.Stems.Low - 1, d.Stems.Low, At(d.Stems, 1, 2), d.Stems.High, d.Stems.High + 1, double.NaN };
                double[] basal = { d.Basal.Low - 0.5, d.Basal.Low, At(d.Basal, 1, 2), d.Basal.High, d.Basal.High + 0.5, double.NaN };
                double[] tops = { d.MinTop - 0.001, d.MinTop, d.MinTop + 10, double.NaN, double.PositiveInfinity };
                int available = 0, missing = 0;
                for (int age = 0; age <= d.Cutoff; age++)
                foreach (double s in stems)
                foreach (double b in basal)
                foreach (double t in tops)
                foreach (SiteMoistureClass moisture in SpeciesMoistureTable.MoistureClasses)
                {
                    if (AssertDelegated(species, age, s, b, t, moisture)) available++;
                    else missing++;
                }
                True(available > 0 && missing > 0, $"{species}: both in-domain ({available}) and out-of-domain ({missing}) cases ran");
                True(Late(species, d.Low - 1, d.Stems.Low, d.Basal.Low, d.MinTop, SiteMoistureClass.NeutralLegacy).PotentialDbhIncrementCm is null,
                    species + " stays unavailable below the window, exactly as the existing evaluator");
            }
        });

        TestRunner.Add(Suite, "GROW-05 the cutoff anchor is the delegated zero-crowding NeutralLegacy value, bit-exact (0.20 / 0.20 / 0.25 within 1e-12)", () =>
        {
            foreach (var (species, expected) in new[] { (Pine, 0.20), (Alder, 0.20), (Birch, 0.25) })
            {
                Domain d = DomainOf(species);
                double anchor = Nominal(species, d.Cutoff);
                BitEq(DelegatedNominal(species, d.Cutoff), anchor, species + " continuation == delegated value at the cutoff");
                ApproxEq(expected, anchor, 1e-12, species + " anchor");
                LateLifeGrowthResult atCutoff = Late(species, d.Cutoff, d.Stems.Low, d.Basal.Low, d.MinTop, SiteMoistureClass.NeutralLegacy);
                BitEq(anchor, Val(atCutoff.PotentialDbhIncrementCm, "potential"), species + " evaluator at the cutoff");
                BitEq(anchor, Val(atCutoff.SiteAdjustedDbhIncrementCm, "site-adjusted"), species + " NeutralLegacy leaves it unchanged");
                True(atCutoff.IsWithinCalibrationWindow, species + " the cutoff year is still inside the window");
            }

            // IEEE-754 detail, not a model jump: the existing linear window gives exactly 0.20 for pine and 0.25 for birch,
            // but 0.95 - 1.0 * (0.95 - 0.20) for alder lands two ULPs below 0.20; the continuation reuses those exact bits.
            BitEq(0.20, Nominal(Pine, 60), "pine anchor is the double nearest 0.20");
            BitEq(0.25, Nominal(Birch, 70), "birch anchor is exactly 0.25");
            BitEq(Math.BitDecrement(Math.BitDecrement(0.20)), Nominal(Alder, 60), "alder anchor is two ULPs below 0.20");
            Eq("0.19999999999999996", Nominal(Alder, 60).ToString("R", CultureInfo.InvariantCulture), "alder anchor text");
        });

        TestRunner.Add(Suite, "GROW-06 C1 continuity: the decay rate k = -windowSlope / A reproduces the existing window slope at the cutoff", () =>
        {
            foreach (var (species, expectedSlope) in new[] { (Pine, -0.008), (Alder, -0.75 / 55), (Birch, -0.60 / 65) })
            {
                Domain d = DomainOf(species);
                double anchor = Nominal(species, d.Cutoff);
                double slope = LateLifeGrowth.WindowSlopeCmPerYear(species);
                ApproxEq(expectedSlope, slope, 1e-15, species + " window slope");
                BitEq((anchor - DelegatedNominal(species, d.Low)) / (d.Cutoff - d.Low), slope, species + " slope is the delegate's own");
                ApproxEq(slope, anchor - DelegatedNominal(species, d.Cutoff - 1), 1e-15, species + " last in-window year falls by the slope");

                double k = LateLifeGrowth.ExponentialDecayRatePerYear(species);
                True(k > 0.0, species + " k is positive");
                ApproxEq(-slope, k * anchor, 1e-17, species + " d/dage of A*exp(-k(age-cutoff)) at the cutoff equals the window slope");
                for (int age = d.Cutoff + 1; age <= d.Cutoff + 60; age++)
                    BitEq(anchor * Math.Exp(-k * (age - d.Cutoff)), Nominal(species, age), $"{species} age {age} follows A*exp(-k(age-cutoff))");
            }
        });

        TestRunner.Add(Suite, "GROW-07 no cliff: the first year past the cutoff falls by less than the existing window's own final year", () =>
        {
            foreach (string species in Wave2)
            {
                Domain d = DomainOf(species);
                double anchor = Nominal(species, d.Cutoff);
                double continuationDrop = anchor - Nominal(species, d.Cutoff + 1);
                double windowDrop = DelegatedNominal(species, d.Cutoff - 1) - anchor;
                True(continuationDrop > 0.0, species + " keeps declining past the cutoff");
                True(continuationDrop < windowDrop, $"{species} drop {continuationDrop:R} < in-window final-year drop {windowDrop:R}");
                True(continuationDrop < -LateLifeGrowth.WindowSlopeCmPerYear(species), species + " drop < |window slope|");
                ApproxEq(1.0 - Math.Exp(-LateLifeGrowth.ExponentialDecayRatePerYear(species)), continuationDrop / anchor, 1e-12,
                    species + " relative first-year drop is 1 - exp(-k)");
            }
        });

        TestRunner.Add(Suite, "GROW-08 beyond the cutoff the increment stays strictly positive and strictly decreasing through age 300", () =>
        {
            foreach (string species in Wave2)
            {
                Domain d = DomainOf(species);
                double previous = Nominal(species, d.Cutoff);
                for (int age = d.Cutoff + 1; age <= 300; age++)
                {
                    double nominal = Nominal(species, age);
                    True(double.IsFinite(nominal) && nominal > 0.0, $"{species} age {age} positive");
                    True(nominal < previous, $"{species} age {age} strictly decreasing");
                    previous = nominal;

                    LateLifeGrowthResult r = Late(species, age, d.Stems.Low, d.Basal.Low, d.MinTop, SiteMoistureClass.NeutralLegacy);
                    BitEq(nominal, Val(r.PotentialDbhIncrementCm, "potential"), $"{species} age {age} evaluator == nominal");
                    BitEq(nominal, Val(r.SiteAdjustedDbhIncrementCm, "site-adjusted"), $"{species} age {age} NeutralLegacy");
                    True(r.IsAvailable && !r.IsWithinCalibrationWindow, $"{species} age {age} available beyond the window");
                }
                True(Nominal(species, 200) > 0.0, species + " still growing at 200");
                Throws<ArgumentOutOfRangeException>(() => Nominal(species, d.Cutoff - 1), species + " nominal continuation starts at the cutoff");
            }
        });

        TestRunner.Add(Suite, "GROW-09 beyond the cutoff crowding uses the window's own rule: max of the two pressures and the species' maximum reduction", () =>
        {
            foreach (string species in Wave2)
            {
                Domain d = DomainOf(species);
                int age = d.Cutoff + 15;
                double nominal = Nominal(species, age);
                double top = d.MinTop + 5;
                double Potential(double stems, double basal) =>
                    Val(Late(species, age, stems, basal, top, SiteMoistureClass.NeutralLegacy).PotentialDbhIncrementCm, "potential");

                BitEq(nominal, Potential(d.Stems.Low, d.Basal.Low), species + " zero crowding keeps the nominal increment");
                BitEq(nominal * (1.0 - d.MaxReduction * 1.0), Potential(d.Stems.High, d.Basal.High), species + " full crowding");
                BitEq(Potential(d.Stems.High, d.Basal.High), Potential(d.Stems.High, d.Basal.Low), species + " stocking alone saturates");
                BitEq(Potential(d.Stems.High, d.Basal.High), Potential(d.Stems.Low, d.Basal.High), species + " basal area alone saturates");

                double previous = double.PositiveInfinity;
                for (int i = 0; i <= 8; i++)
                {
                    double p = Potential(At(d.Stems, i, 8), d.Basal.Low);
                    True(p < previous, $"{species} more stems, less growth (step {i})");
                    previous = p;
                }

                // The same relative crowding response on both sides of the seam.
                double anchor = Nominal(species, d.Cutoff);
                for (int i = 0; i <= 4; i++)
                for (int j = 0; j <= 4; j++)
                {
                    double stems = At(d.Stems, i, 4), basal = At(d.Basal, j, 4);
                    double inWindow = Val(Delegated(species, d.Cutoff, stems, basal, top, SiteMoistureClass.NeutralLegacy).Potential, "delegated") / anchor;
                    ApproxEq(inWindow, Potential(stems, basal) / nominal, 1e-15, $"{species} crowding factor ({i},{j})");
                }
            }

            // Pine's maximum reduction is private in ScotsPineGrowth.cs; the delegate itself shows it is 0.50 at the cutoff.
            Domain pine = DomainOf(Pine);
            BitEq(Nominal(Pine, 60) * 0.5, Val(Delegated(Pine, 60, pine.Stems.High, pine.Basal.High, pine.MinTop, SiteMoistureClass.NeutralLegacy).Potential,
                "delegated"), "pine full crowding halves the increment in the window too");
        });

        TestRunner.Add(Suite, "GROW-10 beyond the cutoff the moisture growth multiplier is applied exactly once, and NeutralLegacy is bit-exact 1.0", () =>
        {
            foreach (string species in Wave2)
            {
                Domain d = DomainOf(species);
                int age = d.Cutoff + 25;
                double stems = At(d.Stems, 1, 2), basal = At(d.Basal, 1, 4), top = d.MinTop + 3;
                double potential = Val(Late(species, age, stems, basal, top, SiteMoistureClass.NeutralLegacy).PotentialDbhIncrementCm, "potential");
                BitEq(1.0, SpeciesMoistureTable.Cell(species, SiteMoistureClass.NeutralLegacy).Response.GrowthMultiplier, species + " NeutralLegacy is 1.0");
                int reducedClasses = 0;
                foreach (SiteMoistureClass moisture in SpeciesMoistureTable.MoistureClasses)
                {
                    LateLifeGrowthResult r = Late(species, age, stems, basal, top, moisture);
                    MoistureCell cell = SpeciesMoistureTable.Cell(species, moisture);
                    double multiplier = cell.Response.GrowthMultiplier;
                    BitEq(potential, Val(r.PotentialDbhIncrementCm, "potential"), $"{species} {moisture}: moisture never enters the potential");
                    BitEq(potential * multiplier, Val(r.SiteAdjustedDbhIncrementCm, "site-adjusted"), $"{species} {moisture}: potential x multiplier");
                    Eq(cell.Response, r.SiteResponse, $"{species} {moisture}: the table's own site response");
                    if (multiplier != 1.0)
                    {
                        reducedClasses++;
                        True(Val(r.SiteAdjustedDbhIncrementCm, "site-adjusted") != potential * multiplier * multiplier,
                            $"{species} {moisture}: not applied twice");
                    }
                }
                True(reducedClasses > 0, species + " has at least one moisture class below 1.0");
            }
        });

        TestRunner.Add(Suite, "GROW-11 the cutoff seam is exact under crowding and moisture: delegate == continuation formula, then only exp(-k) moves", () =>
        {
            foreach (string species in Wave2)
            {
                Domain d = DomainOf(species);
                double anchor = Nominal(species, d.Cutoff);
                double decay = Math.Exp(-LateLifeGrowth.ExponentialDecayRatePerYear(species));
                for (int i = 0; i <= 4; i++)
                for (int j = 0; j <= 4; j++)
                foreach (SiteMoistureClass moisture in SpeciesMoistureTable.MoistureClasses)
                {
                    double stems = At(d.Stems, i, 4), basal = At(d.Basal, j, 4), top = d.MinTop + 2;
                    string label = $"{species} ({i},{j}) {moisture}";
                    var atCutoff = Delegated(species, d.Cutoff, stems, basal, top, moisture);
                    double multiplier = SpeciesMoistureTable.Cell(species, moisture).Response.GrowthMultiplier;
                    double crowding = Math.Max((stems - d.Stems.Low) / (d.Stems.High - d.Stems.Low),
                                               (basal - d.Basal.Low) / (d.Basal.High - d.Basal.Low));
                    double formula = anchor * (1.0 - d.MaxReduction * crowding);
                    BitEq(formula, Val(atCutoff.Potential, "delegated potential"), label + " continuation formula at the cutoff == delegate");
                    BitEq(formula * multiplier, Val(atCutoff.SiteAdjusted, "delegated site-adjusted"), label + " site-adjusted at the cutoff");

                    LateLifeGrowthResult after = Late(species, d.Cutoff + 1, stems, basal, top, moisture);
                    ApproxEq(decay, Val(after.PotentialDbhIncrementCm, "potential") / formula, 1e-12, label + " potential ratio across the seam");
                    ApproxEq(decay, Val(after.SiteAdjustedDbhIncrementCm, "site-adjusted") / (formula * multiplier), 1e-12,
                        label + " site-adjusted ratio across the seam");
                }
            }
        });

        TestRunner.Add(Suite, "GROW-12 beyond the cutoff the window's own stocking, basal-area and top-height guards still apply: outside them the increment is null", () =>
        {
            foreach (string species in Wave2)
            {
                Domain d = DomainOf(species);
                int age = d.Cutoff + 10;
                double s = At(d.Stems, 1, 2), b = At(d.Basal, 1, 2), t = d.MinTop + 4;
                (double Stems, double Basal, double Top, string Label)[] outside =
                {
                    (d.Stems.Low - 1, b, t, "stems below"), (d.Stems.High + 1, b, t, "stems above"), (double.NaN, b, t, "stems NaN"),
                    (double.PositiveInfinity, b, t, "stems +inf"), (s, d.Basal.Low - 0.001, t, "basal area below"),
                    (s, d.Basal.High + 0.001, t, "basal area above"), (s, double.NaN, t, "basal area NaN"),
                    (s, b, d.MinTop - 0.001, "top height below"), (s, b, double.NaN, "top height NaN"),
                    (s, b, double.PositiveInfinity, "top height +inf"), (s, b, double.NegativeInfinity, "top height -inf"),
                };
                foreach (var c in outside)
                {
                    LateLifeGrowthResult r = Late(species, age, c.Stems, c.Basal, c.Top, SiteMoistureClass.SeasonallyWetAerated);
                    True(r.PotentialDbhIncrementCm is null && r.SiteAdjustedDbhIncrementCm is null, $"{species} {c.Label}: null increments");
                    True(!r.IsAvailable && !r.IsWithinCalibrationWindow, $"{species} {c.Label}: unavailable");
                    Eq(SpeciesMoistureTable.Cell(species, SiteMoistureClass.SeasonallyWetAerated).Response, r.SiteResponse,
                        $"{species} {c.Label}: still reports the site response");
                    True(Late(species, d.Cutoff, c.Stems, c.Basal, c.Top, SiteMoistureClass.SeasonallyWetAerated).PotentialDbhIncrementCm is null,
                        $"{species} {c.Label}: the same input is also null inside the window");
                }
                foreach (var (stems, basal) in new[] { (d.Stems.Low, d.Basal.Low), (d.Stems.High, d.Basal.High), (d.Stems.Low, d.Basal.High) })
                    True(Late(species, age, stems, basal, d.MinTop, SiteMoistureClass.NeutralLegacy).IsAvailable,
                        $"{species} inclusive domain edge ({stems}, {basal}, {d.MinTop}) stays available");
            }
        });

        TestRunner.Add(Suite, "GROW-13 only the three Wave 2 species are served: sitka-spruce, sessile-oak, european-beech and unknown ids are refused", () =>
        {
            Eq("common-alder,downy-birch,scots-pine",
                string.Join(",", LateLifeGrowth.SupportedSpecies.OrderBy(s => s, StringComparer.Ordinal)), "exactly three species");
            foreach (string? bad in new[] { Sitka, Oak, Beech, "norway-spruce", "Scots-Pine", "", null })
            {
                string label = bad is null ? "null" : $"'{bad}'";
                True(!LateLifeGrowth.IsSupportedSpecies(bad), label + " unsupported");
                Throws<ArgumentException>(() => LateLifeGrowth.EvaluateGrowth(new LateLifeGrowthRequest(bad!, 80, 1000, 30, 10, SiteMoistureClass.NeutralLegacy)),
                    label + " growth refused");
                Throws<ArgumentException>(() => LateLifeGrowth.CutoffAgeYears(bad!), label + " cutoff refused");
                Throws<ArgumentException>(() => LateLifeGrowth.NominalIncrementAtOrBeyondCutoff(bad!, 80), label + " nominal refused");
                Throws<ArgumentException>(() => LateLifeGrowth.WindowSlopeCmPerYear(bad!), label + " slope refused");
                Throws<ArgumentException>(() => LateLifeGrowth.ExponentialDecayRatePerYear(bad!), label + " decay rate refused");
            }

            // Cutoffs come from the existing Wave 2 domains rather than being restated.
            Eq((int)FutureSpeciesCatalog.ScotsPine.Readiness.AgeRangeYears!.Value.High, LateLifeGrowth.CutoffAgeYears(Pine), "pine cutoff source");
            Eq((int)BirchAlderEngine.GrowthDomain(Alder).AgeYears.High, LateLifeGrowth.CutoffAgeYears(Alder), "alder cutoff source");
            Eq((int)BirchAlderEngine.GrowthDomain(Birch).AgeYears.High, LateLifeGrowth.CutoffAgeYears(Birch), "birch cutoff source");
            Eq("60,60,70", $"{LateLifeGrowth.CutoffAgeYears(Pine)},{LateLifeGrowth.CutoffAgeYears(Alder)},{LateLifeGrowth.CutoffAgeYears(Birch)}",
                "cutoff ages");
        });
    }
}
