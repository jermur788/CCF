using SitkaP1.Core.NaturalProcesses.Senescence;
using static SitkaP1.Core.Senescence.Tests.Fixtures;
using static SitkaP1.Core.Senescence.Tests.TestRunner;

namespace SitkaP1.Core.Senescence.Tests;

/// <summary>FR-3, FR-4, FR-5 / AC-3: keyed rolls, validation, death semantics and player wording.</summary>
internal static class RollsAndSemanticsTests
{
    private const string Suite = "RollsAndSemantics";

    public static void Register()
    {
        TestRunner.Add(Suite, "ROLL-01 the same request evaluated twice gives an identical result, deaths included", () =>
        {
            SenescenceResult a = Run(12, MixedPopulation(1000));
            SenescenceResult b = Run(12, MixedPopulation(1000));
            True(a.Deaths.Count > 0, "fixture has deaths");
            True(a == b, "results are value-equal");
            Eq(Describe(a), Describe(b), "every field identical");
        });

        TestRunner.Add(Suite, "ROLL-02 a different year (or seed) changes at least one tree's fate", () =>
        {
            List<SenescenceTreeInput> cohort = Cohort(Alder, 200, 1000, "A");
            string year1 = DeathIds(Run(1, cohort));
            True(year1.Length > 0, "year 1 has deaths");
            True(year1 != DeathIds(Run(2, cohort)), "years 1 and 2 kill different trees");
            True(year1 != DeathIds(Run(1, cohort, Seed + 1)), "a different seed kills different trees");
            True(SenescenceEngine.RollUniform(Alder, "A0001", 1, Seed) != SenescenceEngine.RollUniform(Alder, "A0001", 2, Seed),
                "one tree's roll changes with the year");
        });

        TestRunner.Add(Suite, "ROLL-03 the key is prefix + species + '/' + tree id, and changing the species or the tree id changes the draw", () =>
        {
            Eq("SENESCENCE-OLD-AGE-v1-common-alder/T0001", SenescenceStreams.KeyFor(Alder, "T0001"), "key layout");
            Eq("SENESCENCE-OLD-AGE-v1-sessile-oak/LANE-2/T7", SenescenceStreams.KeyFor(Oak, "LANE-2/T7"), "a '/' inside a tree id is kept");
            True(SixSpecies.All(s => !s.Contains('/')), "no species id contains '/', so the key is injective");

            var draws = new HashSet<long>();
            foreach (string species in SixSpecies)
                for (int i = 0; i < 50; i++)
                    draws.Add(BitConverter.DoubleToInt64Bits(SenescenceEngine.RollUniform(species, $"T{i:D4}", 7, Seed)));
            Eq(300, draws.Count, "300 (species, tree) pairs give 300 different draws");

            Throws<ArgumentException>(() => SenescenceStreams.KeyFor("norway-spruce", "T1"), "unsupported species");
            Throws<ArgumentException>(() => SenescenceStreams.KeyFor(null!, "T1"), "null species");
            Throws<ArgumentException>(() => SenescenceStreams.KeyFor(Alder, " "), "blank tree id");
            Throws<ArgumentException>(() => SenescenceStreams.KeyFor(Alder, null!), "null tree id");
            Throws<ArgumentOutOfRangeException>(() => SenescenceEngine.RollUniform(Alder, "T1", 0, Seed), "roll year 0");
        });

        TestRunner.Add(Suite, "ROLL-04 at most one death per tree; a tree dies exactly when its keyed roll is below its hazard; output is in tree-id order", () =>
        {
            List<SenescenceTreeInput> trees = MixedPopulation(1000);
            SenescenceResult r = Run(9, trees);
            True(r.Deaths.Count > 0, "fixture has deaths");
            Eq(r.Deaths.Count, r.Deaths.Select(d => d.TreeId).Distinct(StringComparer.Ordinal).Count(), "at most one death per tree");

            string[] sortedIds = trees.Select(t => t.TreeId).OrderBy(id => id, StringComparer.Ordinal).ToArray();
            True(r.HazardEvaluations.Select(e => e.TreeId).SequenceEqual(sortedIds), "one hazard evaluation per tree, in id order");
            True(r.Deaths.Select(d => d.TreeId).SequenceEqual(r.Deaths.Select(d => d.TreeId).OrderBy(id => id, StringComparer.Ordinal)),
                "deaths in id order");

            var dead = r.Deaths.Select(d => d.TreeId).ToHashSet(StringComparer.Ordinal);
            foreach (SenescenceTreeInput t in trees)
            {
                double hazard = SenescenceHazard.AnnualHazard(t.SpeciesId, t.AgeYears);
                double roll = SenescenceEngine.RollUniform(t.SpeciesId, t.TreeId, 9, Seed);
                Eq(hazard > 0.0 && roll < hazard, dead.Contains(t.TreeId), t.TreeId + " fate follows roll < hazard");
            }
            foreach (SenescenceHazardEvaluation e in r.HazardEvaluations)
                BitEq(SenescenceHazard.AnnualHazard(e.SpeciesId, e.AgeYears), e.AnnualHazard, e.TreeId + " evaluation carries the pure hazard");
        });

        TestRunner.Add(Suite, "ROLL-05 when enabled, duplicate or blank ids, null entries or list, negative ages, year 0 and unknown species are refused", () =>
        {
            SenescenceTreeInput ok = new("T1", Alder, 80);
            Throws<ArgumentException>(() => Run(1, new[] { ok, new SenescenceTreeInput("T1", Oak, 20) }), "duplicate tree id");
            Throws<ArgumentException>(() => Run(1, new[] { ok, ok }), "the same entry twice");
            Throws<ArgumentException>(() => Run(1, new[] { new SenescenceTreeInput(" ", Alder, 80) }), "blank tree id");
            Throws<ArgumentException>(() => Run(1, new[] { new SenescenceTreeInput(null!, Alder, 80) }), "null tree id");
            Throws<ArgumentException>(() => Run(1, new[] { ok, null! }), "null entry");
            Throws<ArgumentNullException>(() => Run(1, null!), "null list");
            Throws<ArgumentOutOfRangeException>(() => Run(1, new[] { new SenescenceTreeInput("T1", Alder, -1) }), "negative age");
            Throws<ArgumentOutOfRangeException>(() => Run(0, new[] { ok }), "year 0");
            Throws<ArgumentException>(() => Run(1, new[] { new SenescenceTreeInput("T1", "alder", 80) }), "unknown species");
            Throws<ArgumentException>(() => Run(1, new[] { new SenescenceTreeInput("T1", null!, 80) }), "null species");
            NoThrow(() => Run(1, new[] { new SenescenceTreeInput("t1", Alder, 80), new SenescenceTreeInput("T1", Alder, 80) }),
                "ids are compared ordinally: t1 and T1 are two trees");
        });

        TestRunner.Add(Suite, "ROLL-06 an old-age death is a biological death: cause old-age, no timber revenue, clears crop-tree marks, never a fell", () =>
        {
            SenescenceResult r = Run(4, Cohort(Alder, 200, 1000, "A"));
            True(r.Deaths.Count > 0, "fixture has deaths");
            var direct = new SenescenceDeathRecord("X1", Oak, 3);
            foreach (SenescenceDeathRecord d in r.Deaths.Append(direct))
            {
                Eq("old-age", d.Cause, d.TreeId + " cause");
                Eq(SenescenceText.CauseOldAge, d.Cause, d.TreeId + " cause constant");
                True(d.BiologicalDeath, d.TreeId + " biological death");
                Eq(0m, d.TimberRevenueEur, d.TreeId + " no timber revenue");
                True(d.ClearsCropTreeMarks, d.TreeId + " clears crop-tree marks");
                True(!d.CountsAsFell, d.TreeId + " is not a fell");
            }
            True(r.Deaths.All(d => d.Year == 4 && d.SpeciesId == Alder), "deaths carry the request year and the tree's species");

            // Cause and wording are fixed by the type: the only public constructor takes the tree id, species and year.
            var ctors = typeof(SenescenceDeathRecord).GetConstructors();
            Eq(1, ctors.Length, "one public constructor");
            True(ctors[0].GetParameters().Select(p => p.ParameterType).SequenceEqual(new[] { typeof(string), typeof(string), typeof(int) }),
                "SenescenceDeathRecord(string TreeId, string SpeciesId, int Year)");
        });

        TestRunner.Add(Suite, "ROLL-07 the player-facing cause is exactly \"Died of old age\": plain words, no tags, calibration or disclaimer", () =>
        {
            const string text = "Died of old age";
            Eq(text, SenescenceText.PlayerCauseOldAge, "constant");
            foreach (SenescenceDeathRecord d in Run(4, Cohort(Birch, 200, 1000, "B")).Deaths)
                Eq(text, d.PlayerCauseText, d.TreeId + " wording");
            foreach (string banned in new[] { "[", "]", "calibration", "disclaimer", "[C]", "[E]", "[T]", "[I]", "hazard", "probability", "%" })
                True(!SenescenceText.PlayerCauseOldAge.Contains(banned, StringComparison.OrdinalIgnoreCase), "no '" + banned + "'");
            True(SenescenceText.PlayerCauseOldAge.All(c => char.IsLetter(c) || c == ' '), "letters and spaces only");
        });

        TestRunner.Add(Suite, "ROLL-08 residual mortality is modest: 1,000 age-200 alder lose between 1 and 99 in a year, and a cohort aged 70 keeps veterans to 150", () =>
        {
            int deaths = Run(1, Cohort(Alder, 200, 1000, "A")).Deaths.Count;
            True(deaths > 0 && deaths < 100, $"{deaths} deaths from 1,000 age-200 alder (expected about 30)");

            // Ages 70..150 inclusive, one evaluation per year, survivors age by one year.
            List<SenescenceTreeInput> alive = Cohort(Alder, 70, 1000, "V");
            for (int year = 1; year <= 81; year++)
            {
                var dead = Run(year, alive).Deaths.Select(d => d.TreeId).ToHashSet(StringComparer.Ordinal);
                alive = alive.Where(t => !dead.Contains(t.TreeId)).Select(t => t with { AgeYears = t.AgeYears + 1 }).ToList();
            }
            True(alive.Count > 50 && alive.Count < 600, $"{alive.Count} of 1,000 alder survive from 70 to 150 (expected about 140)");
        });

        TestRunner.Add(Suite, "ROLL-09 caller collections are snapshotted by plain enumeration: later mutation or a hostile CopyTo cannot change requests or results", () =>
        {
            // Fixture self-check: on this SDK the hostile collection really captures LINQ's destination, so the attack is live.
            var probe = new RetainingCollection<int>(new[] { 1, 2, 3 });
            int[] linqCopy = probe.ToArray();
            probe.OverwriteCaptured(99);
            True(probe.CopyToCalls == 1 && linqCopy[0] == 99, "Enumerable.ToArray hands its destination to CopyTo (attack is real)");

            var hostile = new RetainingCollection<SenescenceTreeInput>(Cohort(Alder, 200, 50, "H"));
            var request = new SenescenceRequest(3, Seed, SenescenceModelVersion.OldAgeV1, hostile);
            SenescenceRequest copy = request with { };
            var hostileForInit = new RetainingCollection<SenescenceTreeInput>(Cohort(Alder, 200, 50, "H"));
            SenescenceRequest replaced = request with { Trees = hostileForInit };
            SenescenceResult result = SenescenceEngine.Evaluate(request);
            string baseline = Describe(result);

            var bogus = new SenescenceTreeInput("ZZZ", "not-a-species", -1);
            hostile.OverwriteCaptured(bogus);
            hostileForInit.OverwriteCaptured(bogus);
            Eq(0, hostile.CopyToCalls + hostileForInit.CopyToCalls, "the contracts never call the caller's CopyTo");
            True(request.Trees.All(t => t.TreeId.StartsWith('H')) && copy.Trees.All(t => t.TreeId.StartsWith('H')) &&
                 replaced.Trees.All(t => t.TreeId.StartsWith('H')), "request, with-copy and init replacement unchanged");
            True(request == copy && request == replaced, "request and copies still equal");
            Eq(baseline, Describe(result), "result unchanged");
            Eq(baseline, Describe(SenescenceEngine.Evaluate(request)), "re-running the request gives the same result");
            Eq(baseline, Describe(SenescenceEngine.Evaluate(replaced)), "init replacement gives the same result");

            List<SenescenceTreeInput> list = Cohort(Alder, 200, 50, "L");
            var fromList = new SenescenceRequest(3, Seed, SenescenceModelVersion.OldAgeV1, list);
            string listBaseline = Describe(SenescenceEngine.Evaluate(fromList));
            list.Clear();
            list.Add(bogus);
            Eq(listBaseline, Describe(SenescenceEngine.Evaluate(fromList)), "clearing the caller's list changes nothing");

            True(result.Deaths is not System.Collections.IList && result.Deaths is not ICollection<SenescenceDeathRecord>,
                "deaths are not a writable collection");
            True(result.HazardEvaluations is not System.Collections.IList &&
                 result.HazardEvaluations is not ICollection<SenescenceHazardEvaluation>, "evaluations are not a writable collection");
            True(request.Trees is not System.Collections.IList && request.Trees is not ICollection<SenescenceTreeInput>,
                "request trees are not a writable collection");
        });

        TestRunner.Add(Suite, "ROLL-10 input order never matters: shuffled and reversed inputs give the identical result in tree-id order", () =>
        {
            List<SenescenceTreeInput> trees = MixedPopulation(1000);
            List<SenescenceTreeInput> shuffled = DeterministicShuffle(trees, 0xC0FFEE20261128UL);
            List<SenescenceTreeInput> reversed = Enumerable.Reverse(trees).ToList();
            True(!trees.Select(t => t.TreeId).SequenceEqual(shuffled.Select(t => t.TreeId)), "the shuffle changed the order");

            SenescenceResult a = Run(21, trees);
            SenescenceResult b = Run(21, shuffled);
            SenescenceResult c = Run(21, reversed);
            True(a.Deaths.Count > 0, "fixture has deaths");
            True(a == b && a == c, "results are value-equal");
            Eq(Describe(a), Describe(b), "shuffled input: every field identical");
            Eq(Describe(a), Describe(c), "reversed input: every field identical");
        });
    }
}
