using System.Reflection;
using SitkaP1.Core.NaturalProcesses.Senescence;
using SitkaP1.Core.SpeciesWave2;
using SitkaP1.Core.SpeciesWave2.BirchAlder;
using static SitkaP1.Core.Senescence.Tests.Fixtures;
using static SitkaP1.Core.Senescence.Tests.TestRunner;

namespace SitkaP1.Core.Senescence.Tests;

/// <summary>FR-1 / AC-2: the six-species smoothstep old-age hazard curves.</summary>
internal static class HazardTests
{
    private const string Suite = "Hazard";

    private static double H(string species, int age) => SenescenceHazard.AnnualHazard(species, age);

    private static double MaxOf(int age, params string[] species) => species.Max(s => H(s, age));

    private static double MinOf(int age, params string[] species) => species.Min(s => H(s, age));

    public static void Register()
    {
        TestRunner.Add(Suite, "HAZ-01 the six profiles reuse the SpeciesMoistureTable ids exactly; no second vocabulary, no other id accepted", () =>
        {
            string[] table = SpeciesMoistureTable.SpeciesIds.OrderBy(s => s, StringComparer.Ordinal).ToArray();
            Eq(6, SenescenceHazard.Profiles.Count, "six profiles");
            True(table.SequenceEqual(SenescenceHazard.Profiles.Select(p => p.SpeciesId).OrderBy(s => s, StringComparer.Ordinal)),
                "profile ids are the table ids");
            True(table.SequenceEqual(SenescenceHazard.SupportedSpecies.OrderBy(s => s, StringComparer.Ordinal)),
                "supported ids are the table ids");
            Eq(SpeciesMoistureTable.CommonAlder, SenescenceHazard.CommonAlder.SpeciesId, "alder");
            Eq(SpeciesMoistureTable.DownyBirch, SenescenceHazard.DownyBirch.SpeciesId, "birch");
            Eq(SpeciesMoistureTable.SitkaSpruce, SenescenceHazard.SitkaSpruce.SpeciesId, "sitka");
            Eq(SpeciesMoistureTable.EuropeanBeech, SenescenceHazard.EuropeanBeech.SpeciesId, "beech");
            Eq(SpeciesMoistureTable.ScotsPine, SenescenceHazard.ScotsPine.SpeciesId, "pine");
            Eq(SpeciesMoistureTable.SessileOak, SenescenceHazard.SessileOak.SpeciesId, "oak");
            foreach (string id in table)
            {
                True(SenescenceHazard.IsSupportedSpecies(id), id + " supported");
                Eq(id, SenescenceHazard.GetProfile(id).SpeciesId, id + " profile");
                NoThrow(() => SpeciesMoistureTable.Cell(id, SiteMoistureClass.NeutralLegacy), id + " is a moisture-table id");
            }
            foreach (string bad in new[] { "Common-Alder", "alder", "oak", "sitka", "", " common-alder", "common-alder ", "norway-spruce" })
            {
                True(!SenescenceHazard.IsSupportedSpecies(bad), $"'{bad}' unsupported");
                Throws<ArgumentException>(() => SenescenceHazard.AnnualHazard(bad, 100), $"'{bad}' hazard refused");
                Throws<ArgumentException>(() => SenescenceHazard.GetProfile(bad), $"'{bad}' profile refused");
            }
            True(!SenescenceHazard.IsSupportedSpecies(null), "null unsupported");
            Throws<ArgumentException>(() => SenescenceHazard.AnnualHazard(null!, 100), "null id refused");
        });

        TestRunner.Add(Suite, "HAZ-02 hazard is bit-exact +0.0 at and below onset for all six species; negative ages are refused", () =>
        {
            foreach (SenescenceSpeciesProfile p in SenescenceHazard.Profiles)
            {
                for (int age = 0; age <= p.OnsetAgeYears; age++)
                    Eq(0L, BitConverter.DoubleToInt64Bits(H(p.SpeciesId, age)), $"{p.SpeciesId} age {age}");
                Throws<ArgumentOutOfRangeException>(() => SenescenceHazard.AnnualHazard(p.SpeciesId, -1), p.SpeciesId + " age -1");
            }
        });

        TestRunner.Add(Suite, "HAZ-03 the year after onset is positive, at most 0.002 and below 3*max/ramp^2 (zero-slope C1 start: no birthday cliff)", () =>
        {
            foreach (SenescenceSpeciesProfile p in SenescenceHazard.Profiles)
            {
                double first = H(p.SpeciesId, p.OnsetAgeYears + 1);
                double quadraticBound = 3.0 * p.MaximumAnnualHazard / ((double)p.RampYears * p.RampYears);
                True(first > 0.0, p.SpeciesId + " positive after onset");
                True(first <= 0.002, p.SpeciesId + " h(onset+1) <= 0.002");
                True(first < quadraticBound, p.SpeciesId + " starts quadratically (a linear ramp would be max/ramp)");
            }
        });

        TestRunner.Add(Suite, "HAZ-04 every curve is finite, within [0, max] and non-decreasing over ages 0..300, with year-on-year steps <= 0.002", () =>
        {
            foreach (SenescenceSpeciesProfile p in SenescenceHazard.Profiles)
            {
                double previous = H(p.SpeciesId, 0);
                for (int age = 1; age <= 300; age++)
                {
                    double h = H(p.SpeciesId, age);
                    True(double.IsFinite(h) && h >= 0.0 && h <= p.MaximumAnnualHazard, $"{p.SpeciesId} age {age} in range");
                    True(h >= previous, $"{p.SpeciesId} age {age} non-decreasing");
                    True(h - previous <= 0.002, $"{p.SpeciesId} age {age} step <= 0.002");
                    previous = h;
                }
            }
        });

        TestRunner.Add(Suite, "HAZ-05 the locked table: each ceiling equals its declared maximum (<= 0.05), is reached with zero slope and stays flat", () =>
        {
            (string Species, int Onset, int Ramp, double Max)[] locked =
            {
                (Alder, 55, 60, 0.030), (Birch, 60, 55, 0.026), (Sitka, 90, 55, 0.020),
                (Beech, 115, 60, 0.016), (Pine, 150, 70, 0.010), (Oak, 160, 75, 0.009),
            };
            foreach (var row in locked)
            {
                SenescenceSpeciesProfile p = SenescenceHazard.GetProfile(row.Species);
                Eq(row.Onset, p.OnsetAgeYears, row.Species + " onset");
                Eq(row.Ramp, p.RampYears, row.Species + " ramp");
                BitEq(row.Max, p.MaximumAnnualHazard, row.Species + " maximum");
                True(p.MaximumAnnualHazard <= 0.05, row.Species + " ceiling <= 0.05");
                True(p.Evidence.Primary == EvidenceClass.Calibration && p.Evidence.Secondary is null, row.Species + " is labelled Calibration");

                int full = p.OnsetAgeYears + p.RampYears;
                foreach (int age in new[] { full, full + 1, 300, 1000 })
                    BitEq(p.MaximumAnnualHazard, H(row.Species, age), $"{row.Species} age {age} at the ceiling");
                double arrival = H(row.Species, full) - H(row.Species, full - 1);
                True(arrival > 0.0 && arrival < 3.0 * p.MaximumAnnualHazard / ((double)p.RampYears * p.RampYears),
                    row.Species + " arrives at the ceiling with zero slope");
            }

            // Smoothstep midpoints (t = 1/2 gives s = 1/2) and the documented alder quarter-point.
            BitEq(0.015, H(Alder, 85), "alder midpoint");
            BitEq(0.008, H(Beech, 145), "beech midpoint");
            BitEq(0.005, H(Pine, 185), "pine midpoint");
            ApproxEq(0.0046875, H(Alder, 70), 1e-18, "alder h(70)");
        });

        TestRunner.Add(Suite, "HAZ-06 ordinal grouping at ages 120, 150, 200 and 250: min(alder, birch) > max(beech, sitka) > max(pine, oak)", () =>
        {
            foreach (int age in new[] { 120, 150, 200, 250 })
            {
                double pioneers = MinOf(age, Alder, Birch);
                double middle = MaxOf(age, Beech, Sitka);
                double longLived = MaxOf(age, Pine, Oak);
                True(pioneers > middle, $"age {age}: pioneers {pioneers:R} > mid-longevity {middle:R}");
                True(middle > longLived, $"age {age}: mid-longevity {middle:R} > long-lived {longLived:R}");
            }
        });

        TestRunner.Add(Suite, "HAZ-07 onset order alder < birch < sitka < beech < pine < oak; the later-onset species of each pair has the lower ceiling", () =>
        {
            int[] onsets = SixSpecies.Select(s => SenescenceHazard.GetProfile(s).OnsetAgeYears).ToArray();
            for (int i = 1; i < onsets.Length; i++)
                True(onsets[i - 1] < onsets[i], $"{SixSpecies[i - 1]} onset {onsets[i - 1]} < {SixSpecies[i]} onset {onsets[i]}");
            True(SenescenceHazard.Profiles.Select(p => p.SpeciesId).SequenceEqual(SixSpecies), "Profiles are listed in onset order");

            double Ceiling(string s) => SenescenceHazard.GetProfile(s).MaximumAnnualHazard;
            True(Ceiling(Oak) < Ceiling(Pine), "oak ceiling < pine ceiling");
            True(Ceiling(Beech) < Ceiling(Sitka), "beech ceiling < sitka ceiling");
            True(Ceiling(Birch) < Ceiling(Alder), "birch ceiling < alder ceiling");
        });

        TestRunner.Add(Suite, "HAZ-08 pioneers are never wiped out at rotation age 60-70 and a cohort keeps veterans: 70-150 survival in (0.05, 0.60)", () =>
        {
            for (int age = 0; age <= 60; age++)
                Eq(0L, BitConverter.DoubleToInt64Bits(H(Birch, age)), $"birch age {age} is zero");
            for (int age = 61; age <= 70; age++)
                True(H(Alder, age) < 0.005, $"alder age {age} below 0.005");

            // SurvivalProbability(species, a, b) is the product of (1 - h) over ages a..b, both ends included.
            double alder6070 = SenescenceHazard.SurvivalProbability(Alder, 60, 70);
            double alder70150 = SenescenceHazard.SurvivalProbability(Alder, 70, 150);
            double birch70150 = SenescenceHazard.SurvivalProbability(Birch, 70, 150);
            True(alder6070 > 0.973 && alder6070 < 0.975, $"alder 60..70 survival {alder6070:R} is about 0.974");
            True(alder70150 > 0.05 && alder70150 < 0.60, $"alder 70..150 survival {alder70150:R} in (0.05, 0.60)");
            True(birch70150 > 0.05 && birch70150 < 0.60, $"birch 70..150 survival {birch70150:R} in (0.05, 0.60)");
            ApproxEq(0.140, alder70150, 0.001, "alder 70..150 survival as documented");
            ApproxEq(0.192, birch70150, 0.001, "birch 70..150 survival as documented");

            BitEq(1.0, SenescenceHazard.SurvivalProbability(Birch, 0, 60), "no old-age loss before birch onset");
            Throws<ArgumentOutOfRangeException>(() => SenescenceHazard.SurvivalProbability(Alder, 70, 69), "end before start");
            Throws<ArgumentOutOfRangeException>(() => SenescenceHazard.SurvivalProbability(Alder, -1, 10), "negative start");
            Throws<ArgumentException>(() => SenescenceHazard.SurvivalProbability("alder", 70, 150), "unknown species");
        });

        TestRunner.Add(Suite, "HAZ-09 hazard has no crowding input: the API takes only species and age, and a tree's fate is the same alone or in a dense stand", () =>
        {
            MethodInfo annual = typeof(SenescenceHazard).GetMethod(nameof(SenescenceHazard.AnnualHazard))!;
            True(annual.GetParameters().Select(p => p.ParameterType).SequenceEqual(new[] { typeof(string), typeof(int) }),
                "AnnualHazard(string speciesId, int ageYears)");
            foreach (MethodInfo m in typeof(SenescenceHazard).GetMethods(BindingFlags.Public | BindingFlags.Static))
                True(m.GetParameters().All(p => p.ParameterType == typeof(string) || p.ParameterType == typeof(int)),
                    m.Name + " takes no density, growth, moisture or storm input");
            Eq("AgeYears,SpeciesId,TreeId",
                string.Join(",", typeof(SenescenceTreeInput).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Select(p => p.Name).OrderBy(n => n, StringComparer.Ordinal)),
                "a tree input carries only its id, species and age");
            Eq("Model,Seed,Trees,Year",
                string.Join(",", typeof(SenescenceRequest).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Select(p => p.Name).OrderBy(n => n, StringComparer.Ordinal)),
                "a request carries no stand-density field");

            var probes = new List<SenescenceTreeInput>();
            foreach (string species in SixSpecies)
            {
                SenescenceSpeciesProfile p = SenescenceHazard.GetProfile(species);
                probes.Add(new SenescenceTreeInput("PROBE-" + species + "-ramp", species, p.OnsetAgeYears + p.RampYears / 2));
                probes.Add(new SenescenceTreeInput("PROBE-" + species + "-old", species, 300));
            }
            List<SenescenceTreeInput> dense = MixedPopulation(2000).Concat(probes).ToList();
            foreach (int year in new[] { 1, 7, 33 })
            {
                SenescenceResult alone = Run(year, probes);
                SenescenceResult crowded = Run(year, dense);
                foreach (SenescenceTreeInput probe in probes)
                {
                    True(alone.HazardEvaluations.Single(e => e.TreeId == probe.TreeId) ==
                         crowded.HazardEvaluations.Single(e => e.TreeId == probe.TreeId), $"{probe.TreeId} year {year} hazard");
                    Eq(alone.Deaths.Any(d => d.TreeId == probe.TreeId), crowded.Deaths.Any(d => d.TreeId == probe.TreeId),
                        $"{probe.TreeId} year {year} fate");
                }
            }
        });

        TestRunner.Add(Suite, "HAZ-10 alder heart rot stays a timber downgrade: never read by this module and never used as an annual death rate", () =>
        {
            string? dir = FindDir("src", "Core", "NaturalProcesses", "Senescence");
            True(dir is not null, "located the module source");
            foreach (string file in Directory.GetFiles(dir!, "*.cs", SearchOption.AllDirectories))
            {
                string text = File.ReadAllText(file);
                foreach (string api in new[] { "AlderHeartRot", "InspectAlderHeartRot", "HeartRotDowngrade", "GradeTimber" })
                    True(!text.Contains(api, StringComparison.Ordinal), Path.GetFileName(file) + " must not reference " + api);
            }

            // Where heart-rot risk is already high (0.035 at 51 rising to 0.35 at 60), old-age hazard is zero or tiny.
            for (int age = 51; age <= 60; age++)
                True(H(Alder, age) < BirchAlderEngine.AlderHeartRotRiskProbability(age) / 10.0,
                    $"alder age {age}: old-age hazard is not the heart-rot probability");

            List<SenescenceTreeInput> cohort = Cohort(Alder, 60, 1000, "R");
            int oldAgeDeaths = Run(1, cohort).Deaths.Count;
            int heartRotFlags = 0;
            for (ulong stem = 0; stem < 1000; stem++)
                if (BirchAlderEngine.InspectAlderHeartRot(stem, 60))
                    heartRotFlags++;
            True(heartRotFlags > 250, $"heart rot flags {heartRotFlags} of 1,000 age-60 stems for downgrade");
            True(oldAgeDeaths <= 5, $"yet only {oldAgeDeaths} of 1,000 age-60 alder die of old age");
        });
    }
}
