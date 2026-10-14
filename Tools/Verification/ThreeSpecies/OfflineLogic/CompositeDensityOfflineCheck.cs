using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

// Offline checks for COMPONENT-FAITHFUL mixed-stand mortality (density v3). Plain .NET, no Unity.
//
// Compiles the production density sources unchanged (SitkaGrowthModel, MixedDensityModel, its component planner,
// SitkaSubsetDensitySignal) with this file and the reduced simulator. Acceptance numbers are the ones predeclared in
// Docs/Verification/ThreeSpeciesProgramme/CompositeDensityV3Predeclared.md.
//
//   I1  The Sitka component equals a LITERAL PORT of the original controller block run on the Sitka stems alone.
//   I2  It is unchanged by any non-Reference stem; I4 every Sitka victim is a Reference stem.
//   I3  Sitka and generic victims are disjoint and add up; I5 order and class labels do not matter; I6 no broadleaf share, no generic deaths.
//   I7  Replay is exact.
//   C1-C5 Continuity of total and Sitka-component loss across 100 / 99.9 / 99 / 95 / 90 % Sitka, shielding by small broadleaves, the last broadleaf.
//   Reduced-simulator long runs (pure Oak, pure Beech, mixed) against the Trial 2 acceptance.
public static class CompositeDensityOfflineCheck
{
    static int checks, failures;

    static void Check(bool ok, string name, string detail = "")
    {
        checks++;
        Console.WriteLine((ok ? "PASS " : "FAIL ") + name + (detail.Length > 0 ? " " + detail : ""));
        if (!ok) failures++;
    }

    static string F(double v, string fmt = "0.0000") => v.ToString(fmt, CultureInfo.InvariantCulture);

    const float Area = 0.64f;

    static Func<string, double> SitkaRoll(int year) => id => DensityTrajectorySim.Roll("ADULT-MORT-" + id, year, 1);
    static Func<string, double> GenericRoll(int year) => id => DensityTrajectorySim.Roll("ADULT-MORT-GENERIC-" + id, year, 1);

    // ---------- literal port of the ORIGINAL controller block (ApplyAdultDensityMortality, legacy route) ----------
    // Takes the stems the original block would have seen if the stand held only these trees (tree-id order, as FindTrees
    // returns them) and returns the victim ids. Written from the controller source, not from SitkaSubsetDensitySignal.
    static HashSet<string> LegacyBlockVictims(IReadOnlyList<DensityStem> stems, float standAreaHectares, Func<string, double> roll)
    {
        var result = new HashSet<string>();
        var living = new List<DensityStem>(stems);
        living.Sort((a, b) => string.CompareOrdinal(a.TreeId, b.TreeId));
        if (living.Count == 0 || standAreaHectares <= 0f) return result;
        double squaredDbhSum = 0.0;
        foreach (DensityStem t in living) squaredDbhSum += (double)t.DbhCm * t.DbhCm;
        double stemsPerHectare = living.Count / standAreaHectares;
        double quadraticMeanDbhCm = Math.Sqrt(squaredDbhSum / living.Count);
        double relativeDensity = SitkaGrowthModel.RelativeDensity(stemsPerHectare, quadraticMeanDbhCm);
        double densityPressure = SitkaGrowthModel.DensityPressure(relativeDensity);
        if (densityPressure <= 0.0) return result;

        var suppression = new Dictionary<string, double>(living.Count);
        double vulnerabilitySum = 0.0;
        foreach (DensityStem t in living)
        {
            float ci = (float)t.CompetitionIndex;
            float ci50 = 5f;
            double s = 1.0 - 1.0 / (1.0 + ci / ci50);
            suppression[t.TreeId] = s;
            vulnerabilitySum += SitkaGrowthModel.Vulnerability(s);
        }
        double meanVulnerability = vulnerabilitySum / living.Count;

        var victims = new List<DensityStem>();
        var survivors = new List<DensityStem>(living.Count);
        foreach (DensityStem t in living)
        {
            double probability = SitkaGrowthModel.AnnualDeathProbability(densityPressure, suppression[t.TreeId], meanVulnerability);
            if (probability > 0.0 && roll(t.TreeId) < probability) victims.Add(t); else survivors.Add(t);
        }

        double survivorSquares = 0.0;
        foreach (DensityStem t in survivors) survivorSquares += (double)t.DbhCm * t.DbhCm;
        double survivorRd = survivors.Count == 0 ? 0.0
            : SitkaGrowthModel.RelativeDensity(survivors.Count / standAreaHectares, Math.Sqrt(survivorSquares / survivors.Count));
        if (survivorRd > SitkaGrowthModel.MaximumRelativeDensity)
        {
            var order = new List<DensityStem>(survivors);
            order.Sort((a, b) =>
            {
                int bySuppression = suppression[b.TreeId].CompareTo(suppression[a.TreeId]);
                return bySuppression != 0 ? bySuppression : string.CompareOrdinal(a.TreeId, b.TreeId);
            });
            int count = survivors.Count;
            double sum = survivorSquares;
            foreach (DensityStem t in order)
            {
                double rd = SitkaGrowthModel.RelativeDensity(count / standAreaHectares, Math.Sqrt(sum / count));
                if (rd <= SitkaGrowthModel.MaximumRelativeDensity || count <= 1) break;
                count--;
                sum -= (double)t.DbhCm * t.DbhCm;
                victims.Add(t);
            }
        }
        foreach (DensityStem t in victims) result.Add(t.TreeId);
        return result;
    }

    // ---------- fixtures ----------
    static Random Rng(int seed) => new Random(seed);

    static List<DensityStem> RandomSitka(Random r, int n, string prefix = "S")
    {
        var list = new List<DensityStem>();
        double dbhBase = 10 + r.NextDouble() * 70, spread = 3 + r.NextDouble() * 30;
        double ciBase = 0.5 + r.NextDouble() * 6, ciSpread = r.NextDouble() * 10;
        for (int i = 0; i < n; i++)
            list.Add(new DensityStem(prefix + i.ToString("D5"), GrowingSpaceDemandClass.Reference,
                (float)(dbhBase + r.NextDouble() * spread), (float)(ciBase + r.NextDouble() * ciSpread)));
        return list;
    }

    static List<DensityStem> RandomBroadleaves(Random r, int n, string prefix = "B")
    {
        var list = new List<DensityStem>();
        for (int i = 0; i < n; i++)
            list.Add(new DensityStem(prefix + i.ToString("D5"), r.Next(2) == 0 ? GrowingSpaceDemandClass.Intermediate : GrowingSpaceDemandClass.High,
                (float)(4 + r.NextDouble() * 40), (float)(0.5 + r.NextDouble() * 20)));
        return list;
    }

    static List<T> Shuffled<T>(List<T> source, Random r)
    {
        var copy = new List<T>(source);
        for (int i = copy.Count - 1; i > 0; i--) { int j = r.Next(i + 1); (copy[i], copy[j]) = (copy[j], copy[i]); }
        return copy;
    }

    static HashSet<string> Ids(IReadOnlyList<DensityStem> stems, IEnumerable<int> indices) => new HashSet<string>(indices.Select(i => stems[i].TreeId));

    static bool Throws(Action action)
    {
        try { action(); return false; } catch (ArgumentException) { return true; }
    }

    sealed class PlanView
    {
        public List<string> Ids, Generic;
        public DensityComponentPlan Plan;
        public SitkaComponentResult Sitka;
    }

    static PlanView Plan(List<DensityStem> stems, float area, int year, GenericResponseCalibration? calibration = null)
    {
        var sr = SitkaSubsetDensitySignal.Resolve(stems, area, SitkaRoll(year));
        var plan = calibration.HasValue
            ? MixedDensityModel.PlanComponents(stems, area, sr.HazardVictimIndices, sr.BoundaryVictimIndices, GenericRoll(year), calibration.Value)
            : MixedDensityModel.PlanComponents(stems, area, sr.HazardVictimIndices, sr.BoundaryVictimIndices, GenericRoll(year));
        return new PlanView
        {
            Ids = plan.VictimIndices.Select(i => stems[i].TreeId).ToList(),
            Generic = plan.GenericVictimIndices.Select(i => stems[i].TreeId).ToList(),
            Plan = plan, Sitka = sr
        };
    }

    // ---------- I1 .. I7 ----------
    static void Identities()
    {
        var r = Rng(20261014);
        int states = 0, withHazard = 0, withBoundary = 0, matched = 0;
        for (int s = 0; s < 400; s++)
        {
            int n = new[] { 5, 12, 40, 90, 160, 320 }[s % 6];
            float area = s % 2 == 0 ? 0.16f : 0.64f;
            var sitka = RandomSitka(r, n);
            var listed = Shuffled(sitka, r);
            int year = r.Next(1000);
            var result = SitkaSubsetDensitySignal.Resolve(listed, area, SitkaRoll(year));
            var component = Ids(listed, result.HazardVictimIndices.Concat(result.BoundaryVictimIndices));
            var legacy = LegacyBlockVictims(sitka, area, SitkaRoll(year));
            states++;
            if (result.HazardVictimIndices.Count > 0) withHazard++;
            if (result.BoundaryVictimIndices.Count > 0) withBoundary++;
            if (component.SetEquals(legacy)) matched++;
        }
        Check(matched == states && withHazard >= 100 && withBoundary >= 30,
            "I1_SITKA_COMPONENT_EQUALS_LITERAL_PORT_OF_THE_ORIGINAL_CONTROLLER_BLOCK", $"states={states} identical={matched} withHazardVictims={withHazard} withBoundaryVictims={withBoundary}");

        // I2 / I4: adding, removing or changing broadleaves never changes the Sitka component, which only ever takes Reference stems.
        int invariant = 0, onlyReference = 0, trials = 0;
        for (int s = 0; s < 300; s++)
        {
            var sitka = RandomSitka(r, new[] { 30, 80, 200 }[s % 3]);
            float area = s % 2 == 0 ? 0.16f : 0.64f;
            int year = r.Next(1000);
            var alone = SitkaSubsetDensitySignal.Resolve(sitka, area, SitkaRoll(year));
            var aloneIds = Ids(sitka, alone.HazardVictimIndices.Concat(alone.BoundaryVictimIndices));
            var mixed = Shuffled(sitka.Concat(RandomBroadleaves(r, r.Next(0, 60))).ToList(), r);
            var with = SitkaSubsetDensitySignal.Resolve(mixed, area, SitkaRoll(year));
            var withIds = Ids(mixed, with.HazardVictimIndices.Concat(with.BoundaryVictimIndices));
            trials++;
            if (withIds.SetEquals(aloneIds) && Math.Abs(with.RelativeDensity - alone.RelativeDensity) < 1e-9) invariant++;
            if (with.HazardVictimIndices.Concat(with.BoundaryVictimIndices).All(i => mixed[i].Demand == GrowingSpaceDemandClass.Reference)) onlyReference++;
        }
        Check(invariant == trials, "I2_SITKA_COMPONENT_IS_UNCHANGED_BY_ANY_BROADLEAF", $"trials={trials} identical={invariant}");
        Check(onlyReference == trials, "I4_NO_OAK_OR_BEECH_STEM_IS_EVER_A_SITKA_COMPONENT_VICTIM", $"trials={trials}");

        // I3: structure of the combined plan.
        int disjoint = 0, sums = 0, sorted = 0, bounded = 0, planTrials = 0, genericSeen = 0, sitkaSeen = 0, accounting = 0;
        for (int s = 0; s < 400; s++)
        {
            var sitka = RandomSitka(r, new[] { 0, 10, 60, 150 }[s % 4]);
            var broad = RandomBroadleaves(r, new[] { 1, 8, 40, 120 }[(s / 4) % 4]);
            var stems = Shuffled(sitka.Concat(broad).ToList(), r);
            float area = s % 4 == 0 ? 0.64f : 0.16f;
            int year = r.Next(1000);
            var p = Plan(stems, area, year);
            var plan = p.Plan;
            planTrials++;
            if (!plan.SitkaVictimIndices.Intersect(plan.GenericVictimIndices).Any() && plan.VictimIndices.Distinct().Count() == plan.VictimIndices.Count) disjoint++;
            if (plan.VictimIndices.Count == plan.SitkaVictimIndices.Count + plan.GenericVictimIndices.Count && plan.SitkaVictimIndices.Count == p.Sitka.Deaths) sums++;
            var sortedIds = new List<string>(p.Ids); sortedIds.Sort(string.CompareOrdinal);
            if (p.Ids.SequenceEqual(sortedIds)) sorted++;
            if (plan.VictimIndices.Count <= stems.Count && plan.VictimIndices.All(i => i >= 0 && i < stems.Count)) bounded++;
            if (plan.GenericDeaths > 0) genericSeen++;
            if (plan.SitkaVictimIndices.Count > 0) sitkaSeen++;
            if (plan.HazardDeaths + plan.SitkaBoundaryDeaths == plan.VictimIndices.Count && plan.SitkaHazardDeaths == p.Sitka.HazardVictimIndices.Count
                && plan.SitkaBoundaryDeaths == p.Sitka.BoundaryVictimIndices.Count && plan.GenericDeaths == plan.GenericVictimIndices.Count
                && plan.EligibleForGeneric == stems.Count - p.Sitka.Deaths) accounting++;
        }
        Check(disjoint == planTrials && sums == planTrials, "I3_VICTIM_UNIQUENESS_SITKA_AND_GENERIC_SETS_ARE_DISJOINT_AND_ADD_UP", $"states={planTrials} withGenericVictims={genericSeen} withSitkaVictims={sitkaSeen}");
        Check(sorted == planTrials && bounded == planTrials, "I3_VICTIMS_ARE_IN_TREE_ID_ORDER_AND_IN_RANGE");
        Check(accounting == planTrials, "COMPONENT_MORTALITY_ACCOUNTING_IS_CONSISTENT", $"states={planTrials}");
        Check(genericSeen >= 100 && sitkaSeen >= 100, "ACCOUNTING_FIXTURES_EXERCISE_BOTH_COMPONENTS");

        // Wrong inputs are refused: a duplicate victim, an out-of-range victim, a broadleaf listed as a Sitka victim.
        var tiny = new List<DensityStem> { new DensityStem("S1", GrowingSpaceDemandClass.Reference, 30, 1), new DensityStem("B1", GrowingSpaceDemandClass.High, 30, 1) };
        bool refusedDuplicate = Throws(() => MixedDensityModel.PlanComponents(tiny, Area, new List<int> { 0, 0 }, new List<int>(), GenericRoll(1)));
        bool refusedRange = Throws(() => MixedDensityModel.PlanComponents(tiny, Area, new List<int> { 5 }, new List<int>(), GenericRoll(1)));
        bool refusedBroadleaf = Throws(() => MixedDensityModel.PlanComponents(tiny, Area, new List<int>(), new List<int> { 1 }, GenericRoll(1)));
        Check(refusedDuplicate && refusedRange && refusedBroadleaf, "PLANNER_REFUSES_DUPLICATE_OUT_OF_RANGE_AND_BROADLEAF_SITKA_VICTIMS");

        // I5: order and class labels.
        int orderSame = 0, labelSame = 0, orderTrials = 0, labelVictims = 0;
        for (int s = 0; s < 120; s++)
        {
            var stems = RandomSitka(r, 40).Concat(RandomBroadleaves(r, 50)).ToList();
            int year = r.Next(1000);
            var a = Plan(stems, Area, year);
            var b = Plan(Shuffled(stems, r), Area, year);
            orderTrials++;
            if (a.Ids.SequenceEqual(b.Ids) && a.Generic.SequenceEqual(b.Generic)) orderSame++;
            // swap High<->Intermediate on a symmetric broadleaf set: equal numbers of each at equal size keep occupancy identical
            var sym = new List<DensityStem>(); var flip = new List<DensityStem>();
            for (int i = 0; i < 60; i++)
            {
                var d1 = i % 2 == 0 ? GrowingSpaceDemandClass.High : GrowingSpaceDemandClass.Intermediate;
                var d2 = i % 2 == 0 ? GrowingSpaceDemandClass.Intermediate : GrowingSpaceDemandClass.High;
                float ci = 1 + (i * 7 % 17);
                sym.Add(new DensityStem("N" + i.ToString("D3"), d1, 25 + (i % 5) * 3, ci));
                flip.Add(new DensityStem("N" + i.ToString("D3"), d2, 25 + (i % 5) * 3, ci));
            }
            var reference = stems.Where(x => x.Demand == GrowingSpaceDemandClass.Reference).ToList();
            sym.AddRange(reference); flip.AddRange(reference);
            var ps = Plan(sym, 0.04f, year); var pf = Plan(flip, 0.04f, year);
            labelVictims += ps.Generic.Count;
            if (ps.Generic.SequenceEqual(pf.Generic)) labelSame++;
        }
        Check(orderSame == orderTrials, "I5_LISTING_ORDER_DOES_NOT_CHANGE_ANY_VICTIM", $"trials={orderTrials}");
        Check(labelSame == orderTrials && labelVictims >= 5 * orderTrials, "I5_CLASS_LABELS_NEVER_ENTER_THE_GENERIC_ALLOCATION_AT_EQUAL_OCCUPANCY", $"trials={orderTrials} genericVictimsCompared={labelVictims}");

        // I6: no non-Reference occupied space, no generic deaths (even in a very dense stand).
        int none = 0, noneTrials = 0;
        for (int s = 0; s < 100; s++)
        {
            var stems = RandomSitka(r, new[] { 20, 100, 400 }[s % 3]);
            var p = Plan(stems, 0.16f, s);
            noneTrials++;
            if (p.Plan.GenericDeaths == 0 && p.Plan.NonReferenceShare == 0.0 && p.Plan.VictimIndices.Count == p.Sitka.Deaths) none++;
        }
        Check(none == noneTrials, "I6_NO_NON_REFERENCE_SHARE_MEANS_NO_GENERIC_DEATHS", $"trials={noneTrials}");

        // I7: replay.
        var rs = Rng(7);
        var fixture = RandomSitka(rs, 120).Concat(RandomBroadleaves(rs, 120)).ToList();
        var p1 = Plan(fixture, 0.16f, 33); var p2 = Plan(fixture, 0.16f, 33); var p3 = Plan(fixture, 0.16f, 34);
        Check(p1.Ids.SequenceEqual(p2.Ids) && p1.Generic.SequenceEqual(p2.Generic) && !p1.Ids.SequenceEqual(p3.Ids), "I7_REPLAY_IS_EXACT_AND_THE_YEAR_CHANGES_THE_DRAWS");
    }

    // ---------- generic response shape ----------
    static void ResponseShape()
    {
        var cal = GenericResponseCalibration.Default;
        bool zeroBelow = true, monotonic = true, bounded = true;
        double previous = 0.0;
        for (double o = 0.0; o <= 1000.0; o += o < 10 ? 0.005 : 1.0)
        {
            double l = MixedDensityModel.GenericLoss(o, cal);
            if (o <= 1.0 && l != 0.0) zeroBelow = false;
            if (l < previous - 1e-15) monotonic = false;
            if (!(l < cal.MaximumLoss + 1e-12) || double.IsNaN(l)) bounded = false;
            previous = l;
        }
        bool strictlyIncreasing = true;
        for (double o = 1.01; o < 4.0; o += 0.01)   // beyond about 5 the double-precision exponential has saturated at the bound
            if (!(MixedDensityModel.GenericLoss(o + 0.01, cal) > MixedDensityModel.GenericLoss(o, cal))) strictlyIncreasing = false;
        Check(zeroBelow, "GENERIC_RESPONSE_IS_ZERO_AT_AND_BELOW_THE_ONSET");
        Check(MixedDensityModel.GenericLoss(1.0 + 1e-9, cal) < 1e-6, "GENERIC_RESPONSE_IS_CONTINUOUS_AT_THE_ONSET", F(MixedDensityModel.GenericLoss(1.0 + 1e-9, cal), "0.0e+0"));
        Check(monotonic && strictlyIncreasing, "GENERIC_RESPONSE_IS_MONOTONIC_AND_STRICTLY_INCREASING_ABOVE_THE_ONSET");
        Check(bounded, "GENERIC_RESPONSE_IS_BOUNDED_BELOW_THE_MAXIMUM_FOR_ANY_FINITE_OCCUPANCY", $"loss(1000)={F(MixedDensityModel.GenericLoss(1000.0, cal), "0.000000")} max={F(cal.MaximumLoss)}");

        // No hard capacity: a huge occupancy still removes at most the bound, and never every stem.
        var big = new List<DensityStem>();
        for (int i = 0; i < 400; i++) big.Add(new DensityStem("H" + i.ToString("D4"), GrowingSpaceDemandClass.High, 60f, 4.0));
        var plan = Plan(big, 0.16f, 3).Plan;
        Check(plan.Occupancy > 20.0 && plan.GenericDeaths <= Math.Ceiling(cal.MaximumLoss * big.Count) && plan.GenericDeaths < big.Count, "NO_HARD_SPECIES_DENSITY_CAPACITY",
            $"occupancy={F(plan.Occupancy, "0.0")} deaths={plan.GenericDeaths} of {big.Count}");

        // The response sees occupancy only: two stems with the same occupied space but different class give the same occupancy.
        var o1 = new List<DensityStem> { new DensityStem("A", GrowingSpaceDemandClass.High, 90f, 1) };
        var o2 = new List<DensityStem> { new DensityStem("A", GrowingSpaceDemandClass.Intermediate, 90f * (float)Math.Sqrt(2.0 / 1.5), 1) };
        Check(Math.Abs(MixedDensityModel.Occupancy(o1, 0.16f) - MixedDensityModel.Occupancy(o2, 0.16f)) < 1e-6, "OCCUPANCY_IS_THE_ONLY_INPUT_THE_RESPONSE_SEES");
    }

    // ---------- expected values over many yearly draws ----------
    sealed class Stat
    {
        public double TotalDeaths, SitkaDeaths, GenericDeaths, Stems, Sitka;
        public double TotalLoss => TotalDeaths / Math.Max(1.0, Stems);
        public double SitkaLoss => SitkaDeaths / Math.Max(1.0, Sitka);
    }

    static Stat Expect(List<DensityStem> stems, float area, int seeds, Dictionary<int, HashSet<string>> sitkaIdsByYear = null)
    {
        var s = new Stat { Stems = stems.Count * (double)seeds, Sitka = stems.Count(x => x.Demand == GrowingSpaceDemandClass.Reference) * (double)seeds };
        for (int year = 0; year < seeds; year++)
        {
            var p = Plan(stems, area, year);
            s.TotalDeaths += p.Ids.Count;
            s.SitkaDeaths += p.Plan.SitkaVictimIndices.Count;
            s.GenericDeaths += p.Generic.Count;
            if (sitkaIdsByYear != null) sitkaIdsByYear[year] = new HashSet<string>(p.Plan.SitkaVictimIndices.Select(i => stems[i].TreeId));
        }
        return s;
    }

    // ---------- C1, C2: replacement design (1000 stems, Sitka share 100 .. 90 %) ----------
    static List<DensityStem> Replacement(int broadleaves, float dbh, double ci)
    {
        var list = new List<DensityStem>();
        for (int i = 0; i < 1000; i++)
            list.Add(new DensityStem("S" + i.ToString("D5"), i < broadleaves ? GrowingSpaceDemandClass.High : GrowingSpaceDemandClass.Reference, dbh, ci));
        return list;
    }

    static void ContinuityReplacement()
    {
        const int Seeds = 400;
        int stepsChecked = 0, stepFailures = 0, smallStepFailures = 0, nearPureFailures = 0, sitkaNearFailures = 0, exact = 0, exactTrials = 0;
        double worstNear = 0, worstSitkaNear = 0, worstAdjacent = 0, worstAdjacentMargin = 99;
        var table = new List<string>();
        foreach (float dbh in new[] { 15f, 25f, 40f, 60f })
            foreach (double ci in new[] { 1.0, 3.0, 8.0 })
            {
                Stat previous = null; int previousBroad = 0; Stat pure = null;
                var row = new List<string>();
                foreach (int broad in new[] { 0, 1, 10, 50, 100 })
                {
                    var stems = Replacement(broad, dbh, ci);
                    var stat = Expect(stems, Area, Seeds);
                    row.Add($"{(1000 - broad) / 10.0:0.#}%:{F(stat.TotalLoss, "0.0000")}/{F(stat.SitkaLoss, "0.0000")}");
                    // I1 as a measurement at the replacement states: the Sitka victims equal the literal port on the Sitka stems alone.
                    var sitkaOnly = stems.Where(x => x.Demand == GrowingSpaceDemandClass.Reference).ToList();
                    for (int year = 0; year < 40; year++)
                    {
                        var p = Plan(stems, Area, year);
                        var legacy = LegacyBlockVictims(sitkaOnly, Area, SitkaRoll(year));
                        exactTrials++;
                        if (new HashSet<string>(p.Plan.SitkaVictimIndices.Select(i => stems[i].TreeId)).SetEquals(legacy)) exact++;
                    }
                    if (broad == 0) pure = stat;
                    else
                    {
                        double compositionChange = (broad - previousBroad) / 1000.0;
                        double step = Math.Abs(stat.TotalLoss - previous.TotalLoss);
                        stepsChecked++;
                        worstAdjacentMargin = Math.Min(worstAdjacentMargin, (2.0 * compositionChange + 0.005) / Math.Max(1e-9, step));
                        worstAdjacent = Math.Max(worstAdjacent, step);
                        if (step > 2.0 * compositionChange + 0.005) stepFailures++;
                        if (compositionChange <= 0.001 && step > 0.02) smallStepFailures++;
                        if (broad == 1)
                        {
                            double d = Math.Abs(stat.TotalLoss - pure.TotalLoss), ds = Math.Abs(stat.SitkaLoss - pure.SitkaLoss);
                            worstNear = Math.Max(worstNear, d); worstSitkaNear = Math.Max(worstSitkaNear, ds);
                            if (d > 0.005) nearPureFailures++;
                            if (ds > 0.005) sitkaNearFailures++;
                        }
                    }
                    previous = stat; previousBroad = broad;
                }
                table.Add($"dbh{dbh} ci{ci}: total/Sitka-component loss at Sitka share " + string.Join(" ", row));
            }
        foreach (string line in table) Console.WriteLine("CONTINUITY_TABLE " + line);
        Check(exact == exactTrials, "I1_AT_THE_REPLACEMENT_STATES_THE_SITKA_COMPONENT_EQUALS_THE_ORIGINAL_BLOCK_ON_THE_SITKA_STEMS", $"draws={exactTrials}");
        Check(nearPureFailures == 0, "C1_TOTAL_LOSS_AT_99_9_PERCENT_SITKA_MATCHES_PURE", $"worstDifference={F(worstNear)} tolerance=0.0050 states=12");
        Check(stepFailures == 0, "C1_ADJACENT_TOTAL_LOSS_STEPS_ARE_WITHIN_2X_COMPOSITION_CHANGE_PLUS_0_005", $"steps={stepsChecked} worstStep={F(worstAdjacent)} smallestMarginFactor={F(worstAdjacentMargin, "0.00")}");
        Check(sitkaNearFailures == 0, "C2_SITKA_COMPONENT_LOSS_PER_SITKA_STEM_AT_99_9_PERCENT_MATCHES_PURE", $"worstDifference={F(worstSitkaNear)} tolerance=0.0050");
        Check(smallStepFailures == 0, "NO_ROUTE_CLIFF_NO_TOTAL_LOSS_STEP_OVER_0_02_FOR_A_COMPOSITION_STEP_OF_0_001_OR_LESS");
    }

    // ---------- C3, C4, C5: addition design, shielding, the last broadleaf ----------
    static List<DensityStem> OldSitka(Random r, int n, double dbhMin, double dbhSpread, double ciMin, double ciSpread)
    {
        var list = new List<DensityStem>();
        for (int i = 0; i < n; i++)
            list.Add(new DensityStem("S" + i.ToString("D5"), GrowingSpaceDemandClass.Reference, (float)(dbhMin + r.NextDouble() * dbhSpread), (float)(ciMin + r.NextDouble() * ciSpread)));
        return list;
    }

    static List<DensityStem> SmallBroadleaves(Random r, int n)
    {
        var list = new List<DensityStem>();
        for (int i = 0; i < n; i++)
            list.Add(new DensityStem("B" + i.ToString("D5"), i % 2 == 0 ? GrowingSpaceDemandClass.Intermediate : GrowingSpaceDemandClass.High, (float)(5 + r.NextDouble() * 7), (float)(12 + r.NextDouble() * 10)));
        return list;
    }

    static void Shielding()
    {
        const int Seeds = 400;
        var cases = new (string label, int n, double dbhMin, double dbhSpread, double ciMin, double ciSpread)[]
        {
            ("OLD_SITKA_N100_DBH78_98", 100, 78, 20, 2, 8),
            ("MID_SITKA_N150_DBH55_70", 150, 55, 15, 3, 8),
            ("DENSE_SITKA_N230_DBH45_60", 230, 45, 15, 2, 10)
        };
        foreach (var c in cases)
        {
            var r = Rng(99);
            var sitka = OldSitka(r, c.n, c.dbhMin, c.dbhSpread, c.ciMin, c.ciSpread);
            var pureIdsByYear = new Dictionary<int, HashSet<string>>();
            var pure = Expect(sitka, Area, Seeds, pureIdsByYear);
            var allBroad = SmallBroadleaves(Rng(5), 26);
            bool identical = true; double worstExtra = 0; bool neverNegative = true; var rows = new List<string>();
            foreach (int k in new[] { 0, 1, 3, 12, 26 })
            {
                var stems = sitka.Concat(allBroad.Take(k)).ToList();
                var idsByYear = new Dictionary<int, HashSet<string>>();
                var stat = Expect(stems, Area, Seeds, idsByYear);
                for (int year = 0; year < Seeds; year++)
                    if (!idsByYear[year].SetEquals(pureIdsByYear[year])) identical = false;
                double extra = (stat.TotalDeaths - pure.TotalDeaths) / Seeds;
                worstExtra = Math.Max(worstExtra, extra);
                if (extra < -1e-9) neverNegative = false;
                rows.Add($"{k}:Sitka-component loss {F(stat.SitkaLoss, "0.000")} total deaths/yr {F(stat.TotalDeaths / Seeds, "0.00")}");
            }
            Check(identical, $"C3_SHIELDING_{c.label}_SITKA_VICTIMS_ARE_IDENTICAL_WITH_0_1_3_12_26_BROADLEAVES", string.Join("  ", rows));
            Check(neverNegative && worstExtra <= 0.5 + 0.005 * (c.n + 26), $"C4_SHIELDING_{c.label}_BROADLEAVES_ADD_AT_MOST_THE_GENERIC_EXPECTATION", $"worstAddedDeathsPerYear={F(worstExtra, "0.000")} tolerance={F(0.5 + 0.005 * (c.n + 26), "0.000")}");
        }

        // C5: the last broadleaf. Deliberate removal and natural death give the same surviving state: the same stand without it.
        var stand = OldSitka(Rng(321), 100, 78, 20, 2, 8);
        var withLone = stand.Concat(new[] { new DensityStem("B00000", GrowingSpaceDemandClass.High, 30f, 6.0) }).ToList();
        int sitkaSame = 0, maxDiff = 0; double sumDiff = 0;
        for (int year = 0; year < Seeds; year++)
        {
            var a = Plan(withLone, Area, year); var b = Plan(stand, Area, year);
            var sa = new HashSet<string>(a.Plan.SitkaVictimIndices.Select(i => withLone[i].TreeId));
            var sb = new HashSet<string>(b.Plan.SitkaVictimIndices.Select(i => stand[i].TreeId));
            if (sa.SetEquals(sb)) sitkaSame++;
            maxDiff = Math.Max(maxDiff, Math.Abs(a.Ids.Count - b.Ids.Count));
            sumDiff += a.Ids.Count - b.Ids.Count;
        }
        Check(sitkaSame == Seeds, "C5_LAST_BROADLEAF_REMOVAL_OR_DEATH_LEAVES_THE_SITKA_VICTIMS_UNCHANGED", $"seeds={Seeds}");
        Check(maxDiff <= 1 && Math.Abs(sumDiff / Seeds) <= 0.005 * stand.Count, "C5_LAST_BROADLEAF_CHANGES_THE_TOTAL_BY_AT_MOST_ONE_STEM_A_YEAR", $"maxDifference={maxDiff} meanDifference={F(sumDiff / Seeds)}");
    }

    // ---------- the generic component: its expected count and its neutrality ----------
    static void GenericComponent()
    {
        // Expected generic deaths = GenericLossFraction x eligible stems, averaged over many yearly draws.
        var r = Rng(2024);
        var stems = OldSitka(r, 60, 40, 20, 1, 6).Concat(RandomBroadleaves(r, 200)).ToList();
        const int Seeds = 1500;
        double sum = 0, sumSq = 0, expected = 0;
        for (int year = 0; year < Seeds; year++)
        {
            var p = Plan(stems, 0.16f, year);
            sum += p.Plan.GenericDeaths; sumSq += (double)p.Plan.GenericDeaths * p.Plan.GenericDeaths;
            expected = p.Plan.GenericLossFraction * p.Plan.EligibleForGeneric;   // eligible varies a little by year; the last year stands in
        }
        double mean = sum / Seeds, se = Math.Sqrt(Math.Max(1e-12, sumSq / Seeds - mean * mean) / Seeds);
        Check(expected > 3.0 && Math.Abs(mean - expected) <= 4 * se + 0.5, "GENERIC_DEATHS_MATCH_THE_EXPECTED_COUNT_OVER_MANY_YEARS", $"mean={F(mean, "0.000")} expected~{F(expected, "0.000")} se={F(se, "0.000")}");

        // Species neutrality of WHO dies: stems come in pairs of equal size; swapping the two classes inside every pair leaves the
        // occupied space and the non-Reference share exactly as they were, so the generic victims must be exactly the same.
        int same = 0, trials = 0, totalVictims = 0;
        for (int year = 0; year < 120; year++)
        {
            var a = new List<DensityStem>(); var b = new List<DensityStem>();
            for (int pair = 0; pair < 45; pair++)
            {
                float dbh = 30f + (pair * 13 % 20);
                bool referencePair = pair % 5 == 0;
                var first = referencePair ? GrowingSpaceDemandClass.Reference : GrowingSpaceDemandClass.High;
                var second = referencePair ? GrowingSpaceDemandClass.Reference : GrowingSpaceDemandClass.Intermediate;
                float ci1 = 1f + (pair * 5 % 14), ci2 = 2f + (pair * 3 % 11);
                a.Add(new DensityStem("T" + (2 * pair).ToString("D3"), first, dbh, ci1)); a.Add(new DensityStem("T" + (2 * pair + 1).ToString("D3"), second, dbh, ci2));
                b.Add(new DensityStem("T" + (2 * pair).ToString("D3"), second, dbh, ci1)); b.Add(new DensityStem("T" + (2 * pair + 1).ToString("D3"), first, dbh, ci2));
            }
            var pa = MixedDensityModel.PlanComponents(a, 0.04f, new List<int>(), new List<int>(), GenericRoll(year));
            var pb = MixedDensityModel.PlanComponents(b, 0.04f, new List<int>(), new List<int>(), GenericRoll(year));
            trials++;
            totalVictims += pa.GenericVictimIndices.Count;
            if (Math.Abs(pa.Occupancy - pb.Occupancy) < 1e-12 && Math.Abs(pa.NonReferenceShare - pb.NonReferenceShare) < 1e-12
                && pa.GenericVictimIndices.SequenceEqual(pb.GenericVictimIndices)) same++;
        }
        Check(same == trials && totalVictims >= 10 * trials, "GENERIC_ALLOCATION_DEPENDS_ON_COMPETITIVE_POSITION_ONLY_NOT_ON_WHICH_SPECIES_SITS_WHERE", $"trials={trials} genericVictimsCompared={totalVictims}");

        // Among equal-weight non-Reference stems, deaths follow suppression: the more suppressed half dies far more often.
        var graded = new List<DensityStem>();
        for (int i = 0; i < 200; i++) graded.Add(new DensityStem("G" + i.ToString("D4"), GrowingSpaceDemandClass.High, 40f, i < 100 ? 2.0 : 12.0));
        int low = 0, high = 0;
        for (int year = 0; year < 300; year++)
        {
            var p = MixedDensityModel.PlanComponents(graded, 0.16f, new List<int>(), new List<int>(), GenericRoll(year));
            foreach (int v in p.GenericVictimIndices) { if (v < 100) low++; else high++; }
        }
        Check(high > 3 * Math.Max(1, low), "VICTIMS_ARE_THE_MORE_SUPPRESSED_STEMS", $"lowCompetitionDeaths={low} highCompetitionDeaths={high}");
    }

    // ---------- the numbers the signal and the simulator copy from the species assets ----------
    static string AssetValue(string text, string key)
    {
        var m = System.Text.RegularExpressions.Regex.Match(text, "^\\s*" + key + ":\\s*([-0-9.eE]+)\\s*$", System.Text.RegularExpressions.RegexOptions.Multiline);
        return m.Success ? m.Groups[1].Value : null;
    }

    static void AssetGuards()
    {
        string root = Environment.GetEnvironmentVariable("CCF_REPO_ROOT");
        if (string.IsNullOrEmpty(root)) { Check(false, "ASSET_GUARDS_NEED_CCF_REPO_ROOT"); return; }
        var species = new (string file, SimSpecies sim, int demand)[]
        {
            ("SitkaSpruce.asset", SimSpecies.Sitka, 0), ("BeechSpecies.asset", SimSpecies.Beech, 1), ("SessileOak.asset", SimSpecies.Oak, 2)
        };
        foreach (var s in species)
        {
            string text = System.IO.File.ReadAllText(System.IO.Path.Combine(root, "Assets", "ForestPrototype", "Species", s.file));
            double Get(string key) => double.Parse(AssetValue(text, key) ?? "nan", CultureInfo.InvariantCulture);
            // growingSpaceDemand is omitted from the asset when it is the default 0 (Reference)
            string demand = AssetValue(text, "growingSpaceDemand");
            int assetDemand = demand == null ? 0 : int.Parse(demand, CultureInfo.InvariantCulture);
            Check((float)Get("potentialDbhGrowthCmPerYear") == s.sim.PotentialDbhGrowth && (float)Get("maxDbhCm") == s.sim.MaxDbh && (float)Get("ci50") == s.sim.Ci50 && assetDemand == s.demand && (int)s.sim.Demand == s.demand,
                $"SIMULATOR_SPECIES_PARAMETERS_EQUAL_THE_{s.file.Replace(".asset", "").ToUpperInvariant()}_ASSET",
                $"dbhGrowth={Get("potentialDbhGrowthCmPerYear")} maxDbh={Get("maxDbhCm")} ci50={Get("ci50")} demand={assetDemand}");
        }
        string sitkaAsset = System.IO.File.ReadAllText(System.IO.Path.Combine(root, "Assets", "ForestPrototype", "Species", "SitkaSpruce.asset"));
        Check(double.Parse(AssetValue(sitkaAsset, "ci50"), CultureInfo.InvariantCulture) == SitkaSubsetDensitySignal.SitkaCi50, "SITKA_SUBSET_SIGNAL_USES_THE_SITKA_ASSET_CI50", $"asset={AssetValue(sitkaAsset, "ci50")} signal={SitkaSubsetDensitySignal.SitkaCi50}");
    }

    // ---------- reduced-simulator long runs: pure Oak, pure Beech, mixed ----------
    static void LongRuns()
    {
        DensityCalibrationSearch.Acceptance(Check, F);
    }

    public static int Main()
    {
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        string series = Environment.GetEnvironmentVariable("CCF_RECRUITMENT_SERIES");
        if (!string.IsNullOrEmpty(series)) DensityCalibrationSearch.LoadSeriesFile(series);
        AssetGuards();
        Identities();
        ResponseShape();
        ContinuityReplacement();
        Shielding();
        GenericComponent();
        LongRuns();
        Console.WriteLine($"COMPOSITE_DENSITY_{(failures == 0 ? "PASS" : "FAIL")} checks={checks} failures={failures}");
        return failures == 0 ? 0 : 1;
    }
}
