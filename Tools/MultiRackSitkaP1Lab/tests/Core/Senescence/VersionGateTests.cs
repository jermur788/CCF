using SitkaP1.Core.NaturalProcesses.Senescence;
using static SitkaP1.Core.Senescence.Tests.Fixtures;
using static SitkaP1.Core.Senescence.Tests.TestRunner;

namespace SitkaP1.Core.Senescence.Tests;

/// <summary>FR-3 / AC-1: the version gate, the request envelope and the versioned stream prefix.</summary>
internal static class VersionGateTests
{
    private const string Suite = "VersionGate";

    public static void Register()
    {
        TestRunner.Add(Suite, "GATE-01 SenescenceModelVersion is exactly Disabled = 0 and OldAgeV1 = 1, and both defaults are Disabled", () =>
        {
            Eq(SenescenceModelVersion.Disabled, default(SenescenceModelVersion), "default enum value");
            Eq(0, (int)SenescenceModelVersion.Disabled, "Disabled is 0");
            Eq(1, (int)SenescenceModelVersion.OldAgeV1, "OldAgeV1 is 1");
            Eq("Disabled,OldAgeV1", string.Join(",", Enum.GetNames<SenescenceModelVersion>()), "exactly two members");
            var request = new SenescenceRequest(1, Seed, MixedPopulation(10));
            Eq(SenescenceModelVersion.Disabled, request.Model, "the three-argument request defaults to Disabled");
        });

        TestRunner.Add(Suite, "GATE-02 Disabled: 1,000 mixed-age trees including 200+ give zero deaths, zero draws and zero evaluations", () =>
        {
            List<SenescenceTreeInput> trees = MixedPopulation(1000);
            True(trees.Count(t => t.AgeYears >= 200) >= 100, "fixture includes at least 100 trees aged 200+");
            True(SixSpecies.All(s => trees.Any(t => t.SpeciesId == s)), "fixture covers all six species");
            foreach (int year in new[] { 1, 2, 150 })
            foreach (int seed in new[] { 0, Seed, -7 })
            {
                SenescenceResult r = SenescenceEngine.Evaluate(
                    new SenescenceRequest(year, seed, SenescenceModelVersion.Disabled, trees));
                Eq(SenescenceModelVersion.Disabled, r.Model, "model echoed");
                Eq(0, r.Deaths.Count, "no deaths");
                Eq(0, r.RngDrawCount, "no RNG draws");
                Eq(0, r.TreesEvaluated, "no trees evaluated");
                Eq(0, r.HazardEvaluations.Count, "no hazard evaluations");
            }

            // The zero is not vacuous: the same population does lose trees once the model is enabled.
            SenescenceResult enabled = Run(1, trees);
            True(enabled.Deaths.Count > 0, "the same population loses trees when enabled");
            Eq(1000, enabled.RngDrawCount, "enabled draws once per tree");
        });

        TestRunner.Add(Suite, "GATE-03 Disabled skips species and age validation: unknown species, duplicates, negative ages, blanks, null entries and a null list never throw", () =>
        {
            var hostile = new List<SenescenceTreeInput>
            {
                new("T1", "not-a-species", 250),
                new("T1", Sitka, -5),
                new("", "Common-Alder", 99),
                null!,
                new("T2", null!, int.MaxValue),
            };
            SenescenceResult withList = SenescenceEngine.Evaluate(
                new SenescenceRequest(5, Seed, SenescenceModelVersion.Disabled, hostile));
            Eq(0, withList.Deaths.Count + withList.RngDrawCount + withList.TreesEvaluated + withList.HazardEvaluations.Count,
                "hostile list: zero output");
            SenescenceResult withNull = SenescenceEngine.Evaluate(
                new SenescenceRequest(5, Seed, SenescenceModelVersion.Disabled, null!));
            Eq(0, withNull.Deaths.Count + withNull.RngDrawCount + withNull.TreesEvaluated + withNull.HazardEvaluations.Count,
                "null list: zero output");

            // The same unknown species is refused as soon as the model is enabled.
            Throws<ArgumentException>(() => Run(5, new[] { new SenescenceTreeInput("T1", "not-a-species", 250) }),
                "enabled refuses an unknown species");
        });

        TestRunner.Add(Suite, "GATE-04 OldAgeV1 makes exactly one keyed roll per tree, even when every tree carries zero hazard", () =>
        {
            var young = new List<SenescenceTreeInput>();
            int n = 0;
            foreach (SenescenceSpeciesProfile profile in SenescenceHazard.Profiles)
                for (int age = 0; age <= profile.OnsetAgeYears; age += 5)
                    young.Add(new SenescenceTreeInput($"Y{n++:D4}", profile.SpeciesId, age));
            True(SenescenceHazard.Profiles.All(p => young.Any(t => t.SpeciesId == p.SpeciesId && t.AgeYears == p.OnsetAgeYears)),
                "fixture includes every species exactly at onset");

            SenescenceResult r = Run(1, young);
            Eq(young.Count, r.RngDrawCount, "one draw per tree");
            Eq(young.Count, r.TreesEvaluated, "every tree evaluated");
            Eq(young.Count, r.HazardEvaluations.Count, "one hazard evaluation per tree");
            Eq(0, r.Deaths.Count, "zero hazard never kills");
            True(r.HazardEvaluations.All(e => BitConverter.DoubleToInt64Bits(e.AnnualHazard) == 0L), "every hazard is +0.0");

            Eq(1000, Run(3, MixedPopulation(1000)).RngDrawCount, "a mixed population draws once per tree");
            SenescenceResult empty = Run(1, Array.Empty<SenescenceTreeInput>());
            Eq(0, empty.RngDrawCount + empty.TreesEvaluated + empty.Deaths.Count, "an empty enabled request draws nothing");
        });

        TestRunner.Add(Suite, "GATE-05 the request envelope is checked in both modes: null request, Year < 1 and undefined model versions are refused", () =>
        {
            List<SenescenceTreeInput> trees = MixedPopulation(20);
            Throws<ArgumentNullException>(() => SenescenceEngine.Evaluate(null!), "null request");
            foreach (SenescenceModelVersion model in new[] { SenescenceModelVersion.Disabled, SenescenceModelVersion.OldAgeV1 })
            {
                foreach (int year in new[] { 0, -1, int.MinValue })
                    Throws<ArgumentOutOfRangeException>(
                        () => SenescenceEngine.Evaluate(new SenescenceRequest(year, Seed, model, trees)), $"{model} year {year}");
                NoThrow(() => SenescenceEngine.Evaluate(new SenescenceRequest(1, Seed, model, trees)), $"{model} year 1 is accepted");
            }
            foreach (int raw in new[] { 2, -1, 99 })
                Throws<ArgumentOutOfRangeException>(
                    () => SenescenceEngine.Evaluate(new SenescenceRequest(1, Seed, (SenescenceModelVersion)raw, trees)),
                    $"undefined model value {raw}");
        });

        TestRunner.Add(Suite, "GATE-06 the stream prefix is the versioned SENESCENCE-OLD-AGE-v1- and is prefix-free against every existing RNG family", () =>
        {
            const string prefix = "SENESCENCE-OLD-AGE-v1-";
            Eq(prefix, SenescenceStreams.OldAgeTreePrefix, "prefix");

            // Copied verbatim at base b2006767 from src/Core/RngSalts.cs (all 12 Families entries) and from
            // NaturalProcessStreams in src/Core/NaturalProcesses/NaturalProcessRandom.cs (all 10 string constants).
            // Neither file is compiled into this standalone executable.
            string[] existing =
            {
                "ADULT-MORT-", "ADULT-MORT-GENERIC-", "ADULT-MORT-GENERIC-QUOTA-", "PHASE2-MAST-", "PHASE2-PJ-",
                "/vegetation", "/browse", "STAGE5-MAST", "STAGE5-PROMO-", "FOREST-MAKER-",
                "SCOTS-PINE-RECRUIT-", "SCOTS-PINE-ESTABLISH-",
                "STORM-OCCURS-v1", "STORM-SEVERITY-v1", "STORM-DIRECTION-v1", "WINDTHROW-v1-",
                "JAY-DIVERT-v1-", "JAY-ACORN-v1-", "/distance", "/bearing", "/along", "NEARBY-OAK-WOODLAND-",
            };
            Eq(22, existing.Length, "all 22 existing families");
            foreach (string theirs in existing)
                True(!prefix.StartsWith(theirs, StringComparison.Ordinal) && !theirs.StartsWith(prefix, StringComparison.Ordinal),
                    prefix + " vs " + theirs);

            // Every key the engine can build starts with the prefix, so its keys can never land in another family.
            foreach (string species in SixSpecies)
            {
                string key = SenescenceStreams.KeyFor(species, "T0001");
                True(key.StartsWith(prefix, StringComparison.Ordinal), key);
                foreach (string theirs in existing.Where(e => !e.StartsWith('/')))
                    True(!key.StartsWith(theirs, StringComparison.Ordinal), key + " vs " + theirs);
            }
        });
    }
}
