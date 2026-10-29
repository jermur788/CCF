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
//   B1-B6 The basal-area budget of the generic step (Docs/Verification/ThreeSpeciesProgramme/CanopyPulseRootCause.md): exact bound, maximal fill,
//         inert when not needed, never stalls and is not a capacity, reproduces and removes the canopy pulse, class neutral.
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

    // ====================================================================================================================
    // PARITY-03 Q10: heterogeneous, geometry- and class-complete evidence at the pure-Sitka -> one-non-Reference seam.
    //
    // WHY: the replacement sweep above is a valid cliff screen but its fixtures are homogeneous (every stem has the same DBH and
    // the same competition index, so the competition index never decides anything: the dbh/ci table rows are identical) and
    // partly degenerate (dbh15 loses nothing anywhere). It also uses one area, one replacement class, and absolute tolerances
    // (0.005, i.e. 5 stems of 1000) that are 70 % of the annual loss of a stand that loses 0.7 % a year. That sweep is KEPT
    // UNCHANGED above; this section adds evidence, it weakens nothing.
    //
    // WHAT IT IS NOT: no mixed-stand mortality law, no forestry evidence. It detects a route/composition cliff.
    //
    // DESIGN
    //  * Geometry: Legacy40 and Enlarged80 areas are read from the production sources (GeometryGuard), not assumed.
    //  * Class:    the one replaced stem becomes Intermediate (Beech-class demand) or High (Oak-class demand). DBH and
    //              competition index are NOT changed: the state is otherwise identical to the pure one.
    //  * Structure: deterministic heterogeneous Sitka stands (spread of DBH, competition index anti-correlated with size so
    //              suppressed small stems carry the risk), at three relative densities giving a low, a moderate and a
    //              higher annual loss. Stem counts follow the area at the same density, so both geometries carry the same
    //              per-hectare structure. Closed-form low-discrepancy sequences: no seed, no RNG, identical on every runtime.
    //  * Seam:     the pure state is evaluated on the EXACT LEGACY ROUTE (the literal port of the original controller block, which is
    //              what the controller runs for a Reference-only stand). The seam state is evaluated on the species-aware route
    //              (Sitka component + generic component) with exactly one stem replaced, at five positions (highest competition,
    //              median, lowest, largest DBH, smallest DBH). Replacement counts of 0.1 / 1 / 5 / 10 % follow, stems chosen at a
    //              stride across the competition ranking.
    //  * Measure:  expected deaths per year over 400 keyed yearly rolls. The rolls belong to (tree id, year), so a pure state and
    //              the seam state see the SAME draw for every shared stem: differences are structural, not sampling noise. The
    //              only stochastic term is the generic component's one quota draw (standard error <= 0.025 stems).
    //  * Decomposition (exact, from I1): movement = [legacy block on N-k Sitka stems - legacy block on N Sitka stems]
    //              (the PROTECTED legacy model's own response to losing k Sitka stems, inherited and not new)
    //              + expected generic deaths (the only new mechanism). Both are reported.
    // ====================================================================================================================

    // The two authoritative stand areas. GeometryGuard proves these equal what the production sources compute.
    const float Legacy40AreaHa = 0.16f, Enlarged80AreaHa = 0.64f;
    static readonly (string name, float area)[] Geometries = { ("Legacy40", Legacy40AreaHa), ("Enlarged80", Enlarged80AreaHa) };
    static readonly (string name, GrowingSpaceDemandClass demand)[] ReplacementClasses =
        { ("Intermediate(Beech)", GrowingSpaceDemandClass.Intermediate), ("High(Oak)", GrowingSpaceDemandClass.High) };

    static double Frac(double x) => x - Math.Floor(x);

    // Generalised golden-ratio ("R3") low-discrepancy sequence: u_j(i) = frac(0.5 + i * alpha_j). Closed form, no state.
    static readonly double[] R3 = { 0.8191725133961645, 0.6710436067037893, 0.5497004779019703 };

    // One heterogeneous even-aged Sitka-like stand of n stems. d in [0,1] is a triangular size quantile: DBH 25 +- 13 cm.
    // Competition index falls with size: ~0.4 for the largest stems, 7 to 21 for the smallest, scattered by u3 so equal-sized
    // stems do not share one value. IDs follow the generating index, which is unrelated to size.
    static List<DensityStem> HeteroSitka(int n)
    {
        var list = new List<DensityStem>(n);
        for (int i = 0; i < n; i++)
        {
            double u1 = Frac(0.5 + (i + 1) * R3[0]), u2 = Frac(0.5 + (i + 1) * R3[1]), u3 = Frac(0.5 + (i + 1) * R3[2]);
            double d = (u1 + u2) / 2.0;
            float dbh = (float)(25.0 + 13.0 * (2.0 * d - 1.0));
            double ci = 0.4 + 14.0 * (1.0 - d) * (1.0 - d) * (0.5 + u3);
            list.Add(new DensityStem("S" + i.ToString("D5"), GrowingSpaceDemandClass.Reference, dbh, ci));
        }
        return list;
    }

    static double SitkaRd(IReadOnlyList<DensityStem> stems, float area)
    {
        double squares = 0.0;
        foreach (DensityStem s in stems) squares += (double)s.DbhCm * s.DbhCm;
        return SitkaGrowthModel.RelativeDensity(stems.Count / area, Math.Sqrt(squares / stems.Count));
    }

    // The stem count whose Sitka relative density is closest to the target on this area (same per-hectare structure on both areas).
    static readonly Dictionary<(double, float), List<DensityStem>> HeteroCache = new Dictionary<(double, float), List<DensityStem>>();

    // Cached because the search builds thousands of stands; nothing mutates a returned list. The replay check passes fresh: true so that it
    // regenerates the fixture from scratch rather than re-reading a cached copy.
    static List<DensityStem> HeteroSitkaAtRd(double targetRd, float area, bool fresh = false)
    {
        if (!fresh && HeteroCache.TryGetValue((targetRd, area), out var cached)) return cached;
        var built = BuildHeteroSitkaAtRd(targetRd, area);
        if (!fresh) HeteroCache[(targetRd, area)] = built;
        return built;
    }

    static List<DensityStem> BuildHeteroSitkaAtRd(double targetRd, float area)
    {
        int best = 20; double bestGap = double.MaxValue;
        for (int n = 20; n <= 4000; n++)
        {
            double gap = Math.Abs(SitkaRd(HeteroSitka(n), area) - targetRd);
            if (gap < bestGap) { bestGap = gap; best = n; }
        }
        return HeteroSitka(best);
    }

    static List<DensityStem> WithReplaced(List<DensityStem> source, IEnumerable<int> indices, GrowingSpaceDemandClass demand)
    {
        var copy = new List<DensityStem>(source);
        foreach (int i in indices) copy[i] = new DensityStem(copy[i].TreeId, demand, copy[i].DbhCm, copy[i].CompetitionIndex);
        return copy;
    }

    static int ArgBest(List<DensityStem> stems, Func<DensityStem, double> key, bool largest)
    {
        int best = 0;
        for (int i = 1; i < stems.Count; i++)
            if (largest ? key(stems[i]) > key(stems[best]) : key(stems[i]) < key(stems[best])) best = i;
        return best;
    }

    // Index of the stem at the given rank (0 = smallest key) of the stems ordered by key; ties broken by tree id.
    static int AtRank(List<DensityStem> stems, Func<DensityStem, double> key, int rank)
    {
        var order = Enumerable.Range(0, stems.Count).OrderBy(i => key(stems[i])).ThenBy(i => stems[i].TreeId, StringComparer.Ordinal).ToList();
        return order[rank];
    }

    // Expected deaths per year of the EXACT LEGACY ROUTE on a Reference-only stand (what the controller runs for it).
    static double LegacyDeathsPerYear(List<DensityStem> sitkaOnly, float area, int seeds)
    {
        double deaths = 0.0;
        for (int year = 0; year < seeds; year++) deaths += LegacyBlockVictims(sitkaOnly, area, SitkaRoll(year)).Count;
        return deaths / seeds;
    }

    sealed class SeamRow
    {
        public string Geometry, Class, Fixture, Position;
        public int N, K;
        public double Rd, Occupancy;
        public double PureDeaths, SeamDeaths, SitkaDeathsSeam, GenericDeaths, LegacyRemoveDeaths;
        public double ExpectedGeneric, ExpectedGenericStdErr;   // from the public response and occupancy functions, not from the planner's own fields
        public int ExactDraws, ExactMatches;
        public double Tpure => PureDeaths / N;
        public double Tseam => SeamDeaths / N;
        public double Abs => Tseam - Tpure;
        public double Rel => Abs / Math.Max(1e-12, Tpure);
        public double Unit => 1.0 / N;                       // one stem's worth of total-loss fraction
        public double LegacySensitivity => LegacyRemoveDeaths - PureDeaths;   // inherited from the protected Sitka block
        public double SitkaPerStemPure => PureDeaths / N;
        public double SitkaPerStemSeam => SitkaDeathsSeam / Math.Max(1, N - K);
    }

    const int Q10Seeds = 400;

    static SeamRow MeasureSeam(string geometry, float area, string className, GrowingSpaceDemandClass demand, string fixture, List<DensityStem> pure,
        string position, IEnumerable<int> replaced, double pureDeaths)
    {
        var idx = replaced.ToList();
        var seam = WithReplaced(pure, idx, demand);
        var sitkaOnly = seam.Where(x => x.Demand == GrowingSpaceDemandClass.Reference).ToList();
        var row = new SeamRow
        {
            Geometry = geometry, Class = className, Fixture = fixture, Position = position, N = pure.Count, K = idx.Count,
            Rd = SitkaRd(pure, area), Occupancy = MixedDensityModel.Occupancy(seam, area), PureDeaths = pureDeaths
        };
        // Independent oracle for the generic term: response(occupancy) x non-Reference share x stems the Sitka component left standing.
        // Whole-stem rounding is one keyed Bernoulli draw per year, so the variance of the sum is the sum of r(1-r).
        double occupancy = MixedDensityModel.Occupancy(seam, area);
        double space = seam.Sum(MixedDensityModel.StemOccupiedSpace);
        double nonReferenceShare = seam.Where(x => x.Demand != GrowingSpaceDemandClass.Reference).Sum(MixedDensityModel.StemOccupiedSpace) / space;
        double fraction = MixedDensityModel.GenericLoss(occupancy) * nonReferenceShare;
        double total = 0.0, sitka = 0.0, generic = 0.0, expectedGeneric = 0.0, variance = 0.0;
        for (int year = 0; year < Q10Seeds; year++)
        {
            var p = Plan(seam, area, year);
            total += p.Ids.Count; sitka += p.Plan.SitkaVictimIndices.Count; generic += p.Generic.Count;
            double expectedThisYear = fraction * (seam.Count - p.Plan.SitkaVictimIndices.Count);
            expectedGeneric += expectedThisYear;
            double remainder = expectedThisYear - Math.Floor(expectedThisYear + 1e-10);
            variance += remainder * (1.0 - remainder);
            // I1 at this seam state: Sitka victims equal the literal port on the Sitka stems alone (every draw).
            var legacy = LegacyBlockVictims(sitkaOnly, area, SitkaRoll(year));
            row.ExactDraws++;
            if (new HashSet<string>(p.Plan.SitkaVictimIndices.Select(i => seam[i].TreeId)).SetEquals(legacy)) row.ExactMatches++;
        }
        row.SeamDeaths = total / Q10Seeds; row.SitkaDeathsSeam = sitka / Q10Seeds; row.GenericDeaths = generic / Q10Seeds;
        row.ExpectedGeneric = expectedGeneric / Q10Seeds; row.ExpectedGenericStdErr = Math.Sqrt(variance) / Q10Seeds;
        row.LegacyRemoveDeaths = LegacyDeathsPerYear(sitkaOnly, area, Q10Seeds);
        return row;
    }

    static readonly (string name, double rd)[] Q10Fixtures = { ("LowLoss_RD0.72", 0.72), ("Moderate_RD0.80", 0.80), ("Higher_RD0.88", 0.88) };

    static List<SeamRow> Q10Rows;

    static List<SeamRow> BuildQ10Rows()
    {
        var rows = new List<SeamRow>();
        foreach (var (gname, area) in Geometries)
            foreach (var (fname, rd) in Q10Fixtures)
            {
                var pure = HeteroSitkaAtRd(rd, area);
                double pureDeaths = LegacyDeathsPerYear(pure, area, Q10Seeds);
                var seamPositions = new (string name, int index)[]
                {
                    ("highest-CI", ArgBest(pure, s => s.CompetitionIndex, true)),
                    ("median-CI", AtRank(pure, s => s.CompetitionIndex, pure.Count / 2)),
                    ("lowest-CI", ArgBest(pure, s => s.CompetitionIndex, false)),
                    ("largest-DBH", ArgBest(pure, s => s.DbhCm, true)),
                    ("smallest-DBH", ArgBest(pure, s => s.DbhCm, false))
                };
                // two named positions can be the same stem (the largest stem is also the least suppressed): measure each stem once
                var distinct = seamPositions.GroupBy(x => x.index).Select(g => (name: string.Join("=", g.Select(x => x.name)), index: g.Key)).ToList();
                foreach (var (cname, demand) in ReplacementClasses)
                {
                    foreach (var (pname, index) in distinct)
                        rows.Add(MeasureSeam(gname, area, cname, demand, fname, pure, pname, new[] { index }, pureDeaths));
                    // 1 / 10 / 50 / 100 per 1000 stems: replacement counts of 1 % / 5 % / 10 % at a stride across the competition ranking
                    foreach (double fraction in new[] { 0.01, 0.05, 0.10 })
                    {
                        int k = Math.Max(2, (int)Math.Round(fraction * pure.Count));
                        var order = Enumerable.Range(0, pure.Count).OrderBy(i => pure[i].CompetitionIndex).ThenBy(i => pure[i].TreeId, StringComparer.Ordinal).ToList();
                        var picked = Enumerable.Range(0, k).Select(j => order[(int)((j + 0.5) * pure.Count / k)]);
                        rows.Add(MeasureSeam(gname, area, cname, demand, fname, pure, $"stride-{fraction * 100:0}%", picked, pureDeaths));
                    }
                }
            }
        return rows;
    }

    // ---------- Q10 tolerances: VERIFICATION TOLERANCES / GAMEPLAY CALIBRATION. Numerical-continuity bounds, NOT forestry evidence. ----------
    // They answer one question: "does a one-stem change of class move the expected annual deaths by more than a bounded, small amount?"
    // They say nothing about how many broadleaves a real stand loses. Measured on the accepted code (reported in the Q10_SEAM_SUMMARY line)
    // the worst seam movement is 0.12 expected deaths a year and 3.5 % of the annual loss; these bounds sit a factor 2.5 to 4 above it.
    // Every state is deterministic (fixed keyed rolls), so a bound is never "widened to absorb noise": the only random term in the
    // comparison is the generic component's single quota draw (standard error <= 0.025 stems at 400 years), far below SeamAbsoluteStems.
    const double SeamAbsoluteStems = 0.5;       // one stem changing class may move the expected deaths by at most half a stem a year
    const double SeamRelative = 0.10;           // ... and by at most 10 % of the pure stand's own annual deaths, so a low-loss stand cannot hide a jump
    const double SweepSlopeStemsPerStem = 0.25; // replacing k stems may move expected deaths by at most 0.25 stems per replaced stem (+ the seam allowance)
    const double OldAbsoluteTolerance = 0.005;  // the tolerance of C1/C2 above, kept for comparison only

    static bool SeamAbsoluteOk(SeamRow r, double extraDeaths = 0.0) => Math.Abs(r.SeamDeaths - r.PureDeaths + extraDeaths) <= SeamAbsoluteStems + 1e-12;
    static bool SeamRelativeOk(SeamRow r, double extraDeaths = 0.0) => Math.Abs(r.SeamDeaths - r.PureDeaths + extraDeaths) / Math.Max(1e-12, r.PureDeaths) <= SeamRelative + 1e-12;
    static bool SweepOk(SeamRow r) => Math.Abs(r.SeamDeaths - r.PureDeaths) <= SweepSlopeStemsPerStem * r.K + SeamAbsoluteStems + 1e-12;

    static double Pearson(IEnumerable<double> a, IEnumerable<double> b)
    {
        var x = a.ToArray(); var y = b.ToArray();
        double mx = x.Average(), my = y.Average(), sxy = 0, sxx = 0, syy = 0;
        for (int i = 0; i < x.Length; i++) { sxy += (x[i] - mx) * (y[i] - my); sxx += (x[i] - mx) * (x[i] - mx); syy += (y[i] - my) * (y[i] - my); }
        return sxy / Math.Sqrt(sxx * syy);
    }

    static double CoefficientOfVariation(IEnumerable<double> values)
    {
        var v = values.ToArray(); double m = v.Average();
        return Math.Sqrt(v.Sum(x => (x - m) * (x - m)) / v.Length) / m;
    }

    // The areas used above must be what production computes. Parsed from the sources, never assumed.
    static void GeometryGuardQ10()
    {
        string root = Environment.GetEnvironmentVariable("CCF_REPO_ROOT");
        if (string.IsNullOrEmpty(root)) { Check(false, "Q10_GEOMETRY_GUARD_NEEDS_CCF_REPO_ROOT"); return; }
        string fp = System.IO.Path.Combine(root, "Assets", "ForestPrototype");
        string geometry = System.IO.File.ReadAllText(System.IO.Path.Combine(fp, "StandGeometryModel.cs"));
        string controller = System.IO.File.ReadAllText(System.IO.Path.Combine(fp, "ForestEcologyController.cs"));
        var size = System.Text.RegularExpressions.Regex.Match(geometry, @"StandSizeMeters\(int model\)\s*=>\s*model == Enlarged80 \? ([0-9.]+)f : ([0-9.]+)f;");
        var stock = System.Text.RegularExpressions.Regex.Match(geometry, @"StartingStemsPerHectare\s*=\s*([0-9.]+)f;");
        bool formula = controller.Contains("public float StandAreaHectares => standSizeMeters * standSizeMeters / 10000f;");
        bool followsModel = controller.Contains("standSizeMeters = StandGeometryModel.StandSizeMeters(model);");
        bool ids = geometry.Contains("public const int Legacy40 = 0;") && geometry.Contains("public const int Enlarged80 = 1;");
        if (!size.Success || !stock.Success) { Check(false, "Q10_GEOMETRY_AREAS_EQUAL_THE_PRODUCTION_SOURCES", "StandGeometryModel.cs no longer has the expected StandSizeMeters / StartingStemsPerHectare form: update the guard"); return; }
        float e80Size = float.Parse(size.Groups[1].Value, CultureInfo.InvariantCulture), l40Size = float.Parse(size.Groups[2].Value, CultureInfo.InvariantCulture);
        float e80 = e80Size * e80Size / 10000f, l40 = l40Size * l40Size / 10000f;
        float perHa = float.Parse(stock.Groups[1].Value, CultureInfo.InvariantCulture);
        Check(e80 == Enlarged80AreaHa && l40 == Legacy40AreaHa && formula && followsModel && ids,
            "Q10_GEOMETRY_AREAS_EQUAL_THE_PRODUCTION_SOURCES",
            $"Legacy40={l40Size}m->{F(l40, "0.00")}ha Enlarged80={e80Size}m->{F(e80, "0.00")}ha areaFormula={formula} controllerFollowsModel={followsModel} startingStock={perHa}/ha ({Math.Round(perHa * l40)} and {Math.Round(perHa * e80)} trees, context only)");
    }

    static void SeamContinuityQ10()
    {
        GeometryGuardQ10();
        Q10Rows = BuildQ10Rows();
        foreach (SeamRow r in Q10Rows)
            Console.WriteLine($"Q10_SEAM {r.Geometry} {r.Class} {r.Fixture} N={r.N} k={r.K} {r.Position} RD={F(r.Rd, "0.000")} occ={F(r.Occupancy, "0.00")} " +
                $"T_pure={F(r.Tpure, "0.00000")} T_seam={F(r.Tseam, "0.00000")} abs={F(r.Abs, "0.00000")} rel={F(r.Rel * 100, "0.00")}% unit(1/N)={F(r.Unit, "0.00000")} " +
                $"dDeaths={F(r.SeamDeaths - r.PureDeaths, "0.000")} legacySens={F(r.LegacySensitivity, "0.000")} generic={F(r.GenericDeaths, "0.000")} " +
                $"expectedGeneric={F(r.ExpectedGeneric, "0.000")}+-{F(r.ExpectedGenericStdErr, "0.000")} exact={r.ExactMatches}/{r.ExactDraws}");

        var seams = Q10Rows.Where(r => r.K == 1).ToList();
        var sweeps = Q10Rows.Where(r => r.K > 1).ToList();

        // ---- fixtures: heterogeneous, and the competition index really decides who dies ----
        var fixtures = Q10Rows.GroupBy(r => (r.Geometry, r.Fixture)).Select(g => g.First()).ToList();
        bool heterogeneous = true; var hetDetail = new List<string>();
        bool competitionDecides = true; var ciDetail = new List<string>();
        bool lowLossIsLow = true, routesOk = true, pureEndpointOk = true; int pureDraws = 0, pureMatches = 0;
        var pureLoss = new Dictionary<(string, string), double>();
        foreach (var (geometry, area) in Geometries)
            foreach (var (fixture, rd) in Q10Fixtures)
            {
                var pure = HeteroSitkaAtRd(rd, area);
                double cvDbh = CoefficientOfVariation(pure.Select(s => (double)s.DbhCm)), cvCi = CoefficientOfVariation(pure.Select(s => s.CompetitionIndex));
                double correlation = Pearson(pure.Select(s => (double)s.DbhCm), pure.Select(s => s.CompetitionIndex));
                int distinctCi = pure.Select(s => Math.Round(s.CompetitionIndex, 6)).Distinct().Count();
                bool het = pure.Count >= 150 && cvDbh >= 0.15 && cvCi >= 0.6 && correlation <= -0.5 && distinctCi >= 0.9 * pure.Count
                    && pure.Select(s => s.TreeId).Distinct().Count() == pure.Count && pure.All(s => s.Demand == GrowingSpaceDemandClass.Reference);
                heterogeneous &= het;
                hetDetail.Add($"{geometry}/{fixture}:N={pure.Count} cvDbh={F(cvDbh, "0.00")} cvCi={F(cvCi, "0.00")} corr={F(correlation, "0.00")} distinctCi={distinctCi}");

                // Who dies on the exact legacy route: mean competition index of the victims against the stand mean, and the same stand with the index flattened.
                double standMeanCi = pure.Average(s => s.CompetitionIndex);
                var flat = pure.Select(s => new DensityStem(s.TreeId, s.Demand, s.DbhCm, standMeanCi)).ToList();
                double victimCi = 0, flatVictimCi = 0; int victims = 0, flatVictims = 0;
                var ciById = pure.ToDictionary(s => s.TreeId, s => s.CompetitionIndex);
                for (int year = 0; year < Q10Seeds; year++)
                {
                    foreach (string id in LegacyBlockVictims(pure, area, SitkaRoll(year))) { victimCi += ciById[id]; victims++; }
                    foreach (string id in LegacyBlockVictims(flat, area, SitkaRoll(year))) { flatVictimCi += standMeanCi; flatVictims++; }
                }
                double ratio = victims == 0 ? 0 : victimCi / victims / standMeanCi;
                competitionDecides &= victims >= 100 && ratio >= 1.5;   // 1.00 is 'competition decides nothing'; the first bound tried (2.0) was a guess and the measured 1.85 showed it unfounded
                ciDetail.Add($"{geometry}/{fixture}:victims={victims} meanVictimCi/meanStandCi={F(ratio, "0.00")}");

                // Route and endpoint coherence. The controller runs the legacy block for a Reference-only stand; the component planner on the same
                // stand must reproduce it exactly, and the first non-Reference stem must flip the route.
                routesOk &= MixedDensityModel.SelectRoute(true, MixedDensityModel.AnyNonReference(pure)) == DensityRoute.LegacySitkaExact;
                for (int year = 0; year < Q10Seeds; year++)
                {
                    var viaPlanner = new HashSet<string>(Plan(pure, area, year).Ids);
                    pureDraws++;
                    if (viaPlanner.SetEquals(LegacyBlockVictims(pure, area, SitkaRoll(year)))) pureMatches++;
                }
                pureLoss[(geometry, fixture)] = LegacyDeathsPerYear(pure, area, Q10Seeds) / pure.Count;
            }
        foreach (SeamRow r in Q10Rows)
        {
            var one = new[] { new DensityStem("x", r.Class.StartsWith("High") ? GrowingSpaceDemandClass.High : GrowingSpaceDemandClass.Intermediate, 30f, 1.0) };
            routesOk &= MixedDensityModel.SelectRoute(true, MixedDensityModel.AnyNonReference(one)) == DensityRoute.SpeciesAwareOccupancy;
        }
        foreach (var g in Geometries)
            lowLossIsLow &= pureLoss[(g.name, "LowLoss_RD0.72")] > 0.003 && pureLoss[(g.name, "LowLoss_RD0.72")] < 0.01
                && pureLoss[(g.name, "LowLoss_RD0.72")] < pureLoss[(g.name, "Moderate_RD0.80")] && pureLoss[(g.name, "Moderate_RD0.80")] < pureLoss[(g.name, "Higher_RD0.88")];
        Check(heterogeneous, "Q10_FIXTURES_ARE_HETEROGENEOUS_IN_DBH_AND_COMPETITION_INDEX", string.Join("  ", hetDetail));
        Check(competitionDecides, "Q10_THE_COMPETITION_INDEX_DECIDES_WHO_DIES_IN_EVERY_FIXTURE", string.Join("  ", ciDetail) + "  (the old homogeneous fixtures give 1.00 by construction; bound 1.5)");
        Check(lowLossIsLow, "Q10_THE_LOW_LOSS_FIXTURE_LOSES_UNDER_1_PERCENT_A_YEAR_AND_LOSS_RISES_WITH_DENSITY",
            string.Join(" ", pureLoss.Select(kv => $"{kv.Key.Item1}/{kv.Key.Item2}={F(kv.Value, "0.0000")}")) + $"  (the old tolerance {OldAbsoluteTolerance} is {F(OldAbsoluteTolerance / pureLoss[("Legacy40", "LowLoss_RD0.72")] * 100, "0")} % of the Legacy40 low-loss annual loss)");
        Check(routesOk, "Q10_ROUTE_IS_LEGACY_FOR_EVERY_PURE_FIXTURE_AND_OCCUPANCY_FOR_THE_FIRST_NON_REFERENCE_STEM_OF_EITHER_CLASS");
        Check(pureDraws > 0 && pureMatches == pureDraws, "Q10_PURE_ENDPOINT_COMPONENT_PLANNER_EQUALS_THE_EXACT_LEGACY_BLOCK", $"fixtures={Geometries.Length * Q10Fixtures.Length} draws={pureDraws}");

        // ---- coverage: nothing silently dropped ----
        var perFixture = Q10Rows.GroupBy(r => (r.Geometry, r.Class, r.Fixture)).ToList();
        bool covered = Q10Rows.Select(r => r.Geometry).Distinct().Count() == 2 && Q10Rows.Select(r => r.Class).Distinct().Count() == 2
            && perFixture.Count == 2 * 2 * Q10Fixtures.Length
            && perFixture.All(g => g.Count(r => r.K == 1) >= 4 && g.Any(r => r.K >= g.First().N * 0.095) && g.Any(r => r.K > 1 && r.K <= g.First().N * 0.015));
        Check(covered, "Q10_COVERAGE_BOTH_GEOMETRIES_BOTH_CLASSES_THREE_STRUCTURES_FOUR_SEAM_POSITIONS_AND_THE_REPLACEMENT_LADDER",
            $"stateRows={Q10Rows.Count} seamRows={seams.Count} sweepRows={sweeps.Count} cells={perFixture.Count} (Enlarged80 Higher fixture: N=1001, k=1/10/50/100 exactly as the original sweep)");

        // ---- Sitka component preservation: exact at every state ----
        int draws = Q10Rows.Sum(r => r.ExactDraws), matched = Q10Rows.Sum(r => r.ExactMatches);
        Check(draws > 0 && draws == matched, "Q10_SITKA_COMPONENT_EQUALS_THE_LITERAL_ORIGINAL_BLOCK_ON_THE_SITKA_STEMS_AT_EVERY_SEAM_AND_REPLACEMENT_STATE", $"states={Q10Rows.Count} draws={draws}");
        double worstDecomposition = Q10Rows.Max(r => Math.Abs(r.SitkaDeathsSeam - r.LegacyRemoveDeaths));
        Check(worstDecomposition < 1e-9, "Q10_MOVEMENT_DECOMPOSES_EXACTLY_INTO_THE_PROTECTED_LEGACY_ONE_STEM_RESPONSE_PLUS_THE_GENERIC_TERM",
            $"worst|SitkaComponentDeaths - legacyBlockOnSitkaStems|={worstDecomposition:0.0e+0} (the only new mechanism at the seam is the generic term, worst {F(Q10Rows.Where(r => r.K == 1).Max(r => r.GenericDeaths), "0.000")} stems a year)");

        // ---- the independent expectation of the generic term ----
        bool genericMatches = true, genericNeverAbove = true; double worstGenericGap = 0, worstGenericSigma = 0;
        foreach (SeamRow r in Q10Rows)
        {
            double gap = r.GenericDeaths - r.ExpectedGeneric, allowance = 4.0 * r.ExpectedGenericStdErr + 0.01;
            worstGenericGap = Math.Max(worstGenericGap, Math.Abs(gap));
            if (r.K == 1 && Math.Abs(gap) > allowance) genericMatches = false;       // one stem never reaches the basal-area budget: expectation must hold both ways
            if (gap > allowance) genericNeverAbove = false;                           // the budget can only take deaths away, never add them
            if (r.ExpectedGenericStdErr > 0) worstGenericSigma = Math.Max(worstGenericSigma, Math.Abs(gap) / r.ExpectedGenericStdErr);
        }
        Check(genericMatches && genericNeverAbove, "Q10_GENERIC_DEATHS_AT_THE_SEAM_EQUAL_THE_INDEPENDENT_EXPECTATION_RESPONSE_x_SHARE_x_STEMS_LEFT",
            $"worst|measured-expected|={F(worstGenericGap, "0.0000")} stems/yr, worst {F(worstGenericSigma, "0.0")} standard errors; allowance 4 standard errors + 0.01");

        // ---- the continuity criteria ----
        double worstSeamAbs = seams.Max(r => Math.Abs(r.SeamDeaths - r.PureDeaths)), worstSeamRel = seams.Max(r => Math.Abs(r.Rel)), worstSeamAbsT = seams.Max(r => Math.Abs(r.Abs));
        double worstSweep = sweeps.Max(r => Math.Abs(r.SeamDeaths - r.PureDeaths) / (SweepSlopeStemsPerStem * r.K + SeamAbsoluteStems));
        Console.WriteLine($"Q10_SEAM_SUMMARY seamStates={seams.Count} worstAbsDeaths={F(worstSeamAbs, "0.000")} worstAbsLoss={F(worstSeamAbsT, "0.00000")} worstRelative={F(worstSeamRel * 100, "0.00")}% " +
            $"worstSweepFractionOfAllowance={F(worstSweep, "0.00")} perStemSweepMax={F(sweeps.Max(r => Math.Abs(r.SeamDeaths - r.PureDeaths) / r.K), "0.000")}");
        Check(seams.All(r => SeamAbsoluteOk(r)), "Q10_SEAM_ABSOLUTE_ONE_STEM_CHANGES_EXPECTED_ANNUAL_DEATHS_BY_AT_MOST_HALF_A_STEM",
            $"states={seams.Count} worst={F(worstSeamAbs, "0.000")} stems/yr allowance={SeamAbsoluteStems} marginFactor={F(SeamAbsoluteStems / worstSeamAbs, "0.0")} (VERIFICATION TOLERANCE)");
        Check(seams.All(r => SeamRelativeOk(r)), "Q10_SEAM_RELATIVE_ONE_STEM_CHANGES_THE_ANNUAL_LOSS_BY_AT_MOST_10_PERCENT",
            $"states={seams.Count} worst={F(worstSeamRel * 100, "0.00")}% allowance={SeamRelative * 100}% marginFactor={F(SeamRelative / worstSeamRel, "0.0")} (VERIFICATION TOLERANCE)");
        Check(sweeps.All(SweepOk), "Q10_LADDER_REPLACING_k_STEMS_MOVES_EXPECTED_ANNUAL_DEATHS_BY_AT_MOST_0_25_STEMS_PER_STEM_PLUS_THE_SEAM_ALLOWANCE",
            $"states={sweeps.Count} worstFractionOfAllowance={F(worstSweep, "0.00")} marginFactor={F(1.0 / worstSweep, "0.00")} (regression guard on the steepness of the combined response, VERIFICATION TOLERANCE)");

        // ---- the criteria can see what the old tolerance could not ----
        // A synthetic step of +0.004 in annual loss (several tenths of a percent) is below the old 0.005 and so was invisible to C1; every seam state must now reject it.
        int accepted = 0, rejected = 0;
        foreach (SeamRow r in seams)
        {
            double extra = 0.004 * r.N;
            if (Math.Abs(r.Abs + 0.004) <= OldAbsoluteTolerance) accepted++;
            if (!SeamAbsoluteOk(r, extra) || !SeamRelativeOk(r, extra)) rejected++;   // the combined criterion fails when EITHER bound fails
        }
        var lowSeams = seams.Where(r => r.Fixture.StartsWith("LowLoss")).ToList();
        int relativeOnlyCatches = lowSeams.Count(r => SeamAbsoluteOk(r, 0.15 * r.PureDeaths) && !SeamRelativeOk(r, 0.15 * r.PureDeaths));
        Check(accepted == seams.Count && rejected == seams.Count, "Q10_THE_CRITERIA_REJECT_A_SYNTHETIC_0_004_LOSS_JUMP_THAT_THE_OLD_0_005_TOLERANCE_ACCEPTED",
            $"seamStates={seams.Count} oldToleranceWouldAccept={accepted} newCriteriaReject={rejected}; a +15 % jump at a low-loss Legacy40 state is below the half-stem bound but caught by the relative bound in {relativeOnlyCatches} of {lowSeams.Count} low-loss states");

        // ---- replay is exact; the measurement has a stable fingerprint ----
        bool replay = true;
        foreach (var (geometry, area) in Geometries)
            foreach (var (fixture, rd) in Q10Fixtures)
            {
                var pure = HeteroSitkaAtRd(rd, area, fresh: true);
                double pureDeaths = LegacyDeathsPerYear(pure, area, Q10Seeds);
                SeamRow first = Q10Rows.First(r => r.Geometry == geometry && r.Fixture == fixture && r.Class.StartsWith("High") && r.K == 1 && r.Position.StartsWith("highest-CI"));
                SeamRow again = MeasureSeam(geometry, area, first.Class, GrowingSpaceDemandClass.High, fixture, pure, first.Position, new[] { ArgBest(pure, s => s.CompetitionIndex, true) }, pureDeaths);
                replay &= again.SeamDeaths == first.SeamDeaths && again.PureDeaths == first.PureDeaths && again.GenericDeaths == first.GenericDeaths && again.LegacyRemoveDeaths == first.LegacyRemoveDeaths;
            }
        Check(replay, "Q10_REPLAY_IS_BIT_EXACT", "six fixtures re-measured; closed-form fixtures and keyed rolls, no RNG state");
        var sb = new System.Text.StringBuilder();
        foreach (SeamRow r in Q10Rows)
            sb.Append(r.Geometry).Append('|').Append(r.Class).Append('|').Append(r.Fixture).Append('|').Append(r.Position).Append('|').Append(r.N).Append('|').Append(r.K).Append('|')
              .Append(r.PureDeaths.ToString("R", CultureInfo.InvariantCulture)).Append('|').Append(r.SeamDeaths.ToString("R", CultureInfo.InvariantCulture)).Append('|')
              .Append(r.GenericDeaths.ToString("R", CultureInfo.InvariantCulture)).Append('|').Append(r.LegacyRemoveDeaths.ToString("R", CultureInfo.InvariantCulture)).Append('\n');
        using (var sha = System.Security.Cryptography.SHA256.Create())
            Console.WriteLine("Q10_MEASUREMENT_DIGEST sha256=" + BitConverter.ToString(sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(sb.ToString()))).Replace("-", "").ToLowerInvariant() + $" rows={Q10Rows.Count}");
    }


    // ====================================================================================================================
    // PARITY-03 Q10, controller-level fixture: the STATIC PREDICTION for MixedDensityVerification.SeamTransition.
    //
    // The Unity harness builds a regular-grid stand (the same grid, ids and DBH pattern as DensityTrajectorySim.BuildStand), once pure
    // Sitka and once with one Oak or one Beech at the same place and size, and puts each through the real ApplyAdultDensityMortality.
    // This reproduces that stand offline, with the production Hegyi competition, and evaluates the SAME expected-signal criteria. So the
    // criteria the Unity phase will apply were already shown to hold on the production arithmetic, and the Unity run has numbers to
    // reproduce. It does NOT verify the controller: that needs the Unity run, which is queued behind RACK-V2-08 (claim #49).
    // ExpectedSignal below and MixedDensityVerification.ExpectedSignal are the same arithmetic; change both or neither.
    // ====================================================================================================================
    sealed class SignalQ10 { public double Sitka, Generic, Boundary; public double Total => Sitka + Boundary + Generic; }

    static SignalQ10 ExpectedSignal(IReadOnlyList<DensityStem> stems, float area)
    {
        var signal = new SignalQ10();
        SitkaComponentResult sitka = SitkaSubsetDensitySignal.Resolve(stems, area, id => 1.0);   // roll 1.0 never kills: pressure and RD do not depend on the roll
        signal.Boundary = sitka.BoundaryVictimIndices.Count;
        var reference = stems.Where(x => x.Demand == GrowingSpaceDemandClass.Reference).ToList();
        if (reference.Count > 0 && sitka.Pressure > 0.0)
        {
            var suppression = reference.Select(x => 1.0 - 1.0 / (1.0 + (float)x.CompetitionIndex / (float)SitkaSubsetDensitySignal.SitkaCi50)).ToList();
            double meanVulnerability = suppression.Average(x => SitkaGrowthModel.Vulnerability(x));
            signal.Sitka = suppression.Sum(x => SitkaGrowthModel.AnnualDeathProbability(sitka.Pressure, x, meanVulnerability));
        }
        if (MixedDensityModel.AnyNonReference(stems))
        {
            DensityComponentPlan plan = MixedDensityModel.PlanComponents(stems, area, new List<int>(), new List<int>(), id => 1.0);
            signal.Generic = plan.GenericLossFraction * (stems.Count - signal.Sitka - signal.Boundary);
        }
        return signal;
    }

    static int GridCountForRd(float area, double targetRd)
    {
        int best = 20; double bestGap = double.MaxValue;
        for (int count = 20; count <= 6000; count++)
        {
            double squares = 0.0;
            for (int k = 0; k < count; k++) { double d = 24f + ((k * 7919) % 100) / 100f * 10f; squares += d * d; }
            double gap = Math.Abs(SitkaGrowthModel.RelativeDensity(count / area, Math.Sqrt(squares / count)) - targetRd);
            if (gap < bestGap) { bestGap = gap; best = count; }
        }
        return best;
    }

    static List<DensityStem> GridStems(float sideMeters, int count, int probe, SimSpecies probeSpecies)
    {
        var config = new SimConfig { SideMeters = sideMeters, DbhMinCm = 24f, DbhSpreadCm = 10f, StemsPerHectare = count / (sideMeters * sideMeters / 10000f),
            SpeciesOf = k => k == probe ? probeSpecies : SimSpecies.Sitka };
        var trees = DensityTrajectorySim.BuildStand(config);
        if (trees.Count != count) throw new InvalidOperationException("grid stand has " + trees.Count + " stems, expected " + count);
        float[] ci = DensityTrajectorySim.CompetitionIndices(trees);
        return trees.Select((t, i) => new DensityStem(t.Id, t.Species.Demand, t.Dbh, ci[i])).ToList();
    }

    static void ControllerFixturePredictionQ10()
    {
        bool boundaryInactive = true, absOk = true, relOk = true, routeOk = true; int fixtures = 0;
        double worstAbs = 0, worstRel = 0;
        foreach (var (geometry, side, area) in new[] { ("Legacy40", 40f, Legacy40AreaHa), ("Enlarged80", 80f, Enlarged80AreaHa) })
        {
            int count = GridCountForRd(area, 0.78);
            var pureStems = GridStems(side, count, -1, SimSpecies.Sitka);
            SignalQ10 pure = ExpectedSignal(pureStems, area);
            boundaryInactive &= pure.Boundary == 0;
            routeOk &= MixedDensityModel.SelectRoute(true, MixedDensityModel.AnyNonReference(pureStems)) == DensityRoute.LegacySitkaExact;
            foreach (var species in new[] { SimSpecies.Oak, SimSpecies.Beech })
                foreach (int probe in new[] { 0, count / 2, count - 1 })
                {
                    var seamStems = GridStems(side, count, probe, species);
                    SignalQ10 seam = ExpectedSignal(seamStems, area);
                    boundaryInactive &= seam.Boundary == 0;
                    routeOk &= MixedDensityModel.SelectRoute(true, MixedDensityModel.AnyNonReference(seamStems)) == DensityRoute.SpeciesAwareOccupancy;
                    double delta = seam.Total - pure.Total, rel = delta / Math.Max(1e-12, pure.Total);
                    fixtures++;
                    worstAbs = Math.Max(worstAbs, Math.Abs(delta)); worstRel = Math.Max(worstRel, Math.Abs(rel));
                    absOk &= Math.Abs(delta) <= SeamAbsoluteStems + 1e-12;
                    relOk &= Math.Abs(rel) <= SeamRelative + 1e-12;
                    Console.WriteLine($"Q10_CONTROLLER_PREDICTION {geometry} stems={count} probe={probe} {species.Prefix} RD={F(SitkaRd(pureStems, area), "0.000")} " +
                        $"expectedDeaths pure={F(pure.Total, "0.000")} seam={F(seam.Total, "0.000")} (sitka {F(seam.Sitka, "0.000")} + generic {F(seam.Generic, "0.000")}) " +
                        $"delta={F(delta, "0.000")} relative={F(rel * 100, "0.00")}% occupancy={F(MixedDensityModel.Occupancy(seamStems, area), "0.000")}");
                }
        }
        Check(boundaryInactive && routeOk, "Q10_CONTROLLER_FIXTURE_PREDICTION_ROUTES_ARE_LEGACY_THEN_OCCUPANCY_AND_THE_SITKA_BOUNDARY_STAGE_IS_INACTIVE", $"fixtures={fixtures + 2}");
        Check(fixtures == 12 && absOk && relOk, "Q10_CONTROLLER_FIXTURE_PREDICTION_EXPECTED_SIGNAL_IS_CONTINUOUS_ACROSS_THE_0_TO_1_TRANSITION_FOR_OAK_AND_BEECH_ON_BOTH_GEOMETRIES",
            $"fixtures={fixtures} worstAbs={F(worstAbs, "0.000")} stems/yr (allowance {SeamAbsoluteStems}) worstRelative={F(worstRel * 100, "0.00")}% (allowance {SeamRelative * 100}%); this is the prediction the Unity run must reproduce, not controller evidence");
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
            var p = Plan(stems, 0.16f, year, BudgetOff());   // quota arithmetic only: the basal-area budget is tested in CanopyBudget()
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


    // ---------- basal-area budget of the generic step (B1 .. B6) ----------
    static double StemBa(DensityStem s) => Math.PI / 4.0 * (s.DbhCm / 100.0) * (s.DbhCm / 100.0);

    // The production response with the budget switched off (a stem share cannot exceed 1).
    static GenericResponseCalibration BudgetOff() =>
        new GenericResponseCalibration(MixedDensityModel.GenericResponseStrength, MixedDensityModel.GenericMaximumAnnualLoss, MixedDensityModel.GenericResponseExponent, 1.0);

    // Canopy-heavy broadleaf stand: nCanopy large stems with little competition, nSmall recruits with a lot.
    static List<DensityStem> CanopyStand(Random r, int nCanopy, int nSmall, GrowingSpaceDemandClass demand = GrowingSpaceDemandClass.Intermediate)
    {
        var list = new List<DensityStem>();
        for (int i = 0; i < nCanopy; i++)
            list.Add(new DensityStem("C" + i.ToString("D4"), demand, (float)(55 + r.NextDouble() * 20), (float)(0.3 + r.NextDouble() * 2.5)));
        for (int i = 0; i < nSmall; i++)
            list.Add(new DensityStem("R" + i.ToString("D4"), demand, (float)(4 + r.NextDouble() * 3), (float)(8 + r.NextDouble() * 25)));
        return list;
    }

    static void CanopyBudget()
    {
        var def = GenericResponseCalibration.Default;
        var off = BudgetOff();
        double budgetShare = def.BasalAreaBudgetShare;

        // B0 the number is the labelled constant, is validated, and the three-argument constructor takes the production default.
        bool refuses = Throws(() => new GenericResponseCalibration(1.6, 0.5, 2.0, 0.0)) && Throws(() => new GenericResponseCalibration(1.6, 0.5, 2.0, 1.01))
            && Throws(() => new GenericResponseCalibration(1.6, 0.5, 2.0, double.NaN));
        var three = new GenericResponseCalibration(1.6, 0.5, 2.0);
        Check(budgetShare == MixedDensityModel.GenericAnnualBasalAreaBudgetShare && three.BasalAreaBudgetShare == budgetShare && refuses && budgetShare > 0.0 && budgetShare < 0.2,
            "B0_BUDGET_IS_THE_LABELLED_CONSTANT_VALIDATED_AND_THE_DEFAULT", $"budget={F(budgetShare, "0.000")}");

        // B1 exact bound, accounting, maximal fill and inertness over random canopy-heavy and mixed states.
        var r = Rng(20261015);
        int stalled = 0, stalledSubset = 0, states = 0, bound = 0, accounting = 0, maximal = 0, inertSame = 0, inert = 0, bindingDiffer = 0, binding = 0, quotaSame = 0, sitkaSame = 0, fallback = 0, limited = 0;
        for (int s = 0; s < 800; s++)
        {
            int nCanopy = new[] { 6, 14, 24, 40, 80 }[s % 5], nSmall = new[] { 0, 2, 6, 20, 60 }[(s / 5) % 5];
            var stems = CanopyStand(r, nCanopy, nSmall, s % 3 == 0 ? GrowingSpaceDemandClass.High : GrowingSpaceDemandClass.Intermediate);
            if (s % 4 == 0) stems = stems.Concat(RandomSitka(r, 1 + s % 20, "S")).ToList();
            stems = Shuffled(stems, r);
            // area chosen so that the stand is overcrowded (occupancy 1.2 to 2.6), otherwise most states draw no quota at all
            double target = 1.2 + 1.4 * r.NextDouble();
            float area = (float)(stems.Sum(MixedDensityModel.StemOccupiedSpace) / (MixedDensityModel.OccupancyNormalization * target));
            int year = r.Next(1000);
            var on = Plan(stems, area, year);
            var no = Plan(stems, area, year, off);
            var plan = on.Plan;
            if (plan.GenericQuota == 0) continue;
            states++;
            double standBa = stems.Sum(StemBa), budget = budgetShare * standBa;
            var eligible = Enumerable.Range(0, stems.Count).Where(i => !plan.SitkaVictimIndices.Contains(i)).ToList();
            var generic = plan.GenericVictimIndices;
            double spent = generic.Sum(i => StemBa(stems[i]));
            bool nothingFits = eligible.All(i => StemBa(stems[i]) > budget + 1e-12);
            if (spent <= budget + 1e-9 || (generic.Count == 1 && nothingFits)) bound++;
            if (generic.Count == 1 && nothingFits && spent > budget + 1e-9) fallback++;
            if (plan.GenericQuota == plan.GenericDeaths + plan.GenericBudgetLimited && plan.GenericDeaths == generic.Count && Math.Abs(plan.GenericBasalAreaShare - spent / standBa) < 1e-9
                && plan.GenericDeaths <= plan.GenericQuota) accounting++;
            // maximal fill: if the quota was not met, every stem left standing among the eligible ones is too big for what remains of the budget
            if (plan.GenericBudgetLimited == 0 || eligible.Where(i => !generic.Contains(i)).All(i => spent + StemBa(stems[i]) > budget + 1e-12 || (generic.Count == 1 && nothingFits))) maximal++;
            // the quota draw does not depend on the budget; the Sitka component is not touched by it
            if (no.Plan.GenericQuota == plan.GenericQuota) quotaSame++;
            if (no.Plan.SitkaVictimIndices.SequenceEqual(plan.SitkaVictimIndices)) sitkaSame++;
            if (plan.GenericBudgetPassedOver == 0 && plan.GenericBudgetLimited == 0)
            {
                inert++;
                if (no.Generic.SequenceEqual(on.Generic)) inertSame++;
            }
            else if (generic.Count == 1 && nothingFits)
            {
                // never-stall fallback: the highest-priority stem, which is always one of the unbudgeted victims
                stalled++;
                if (no.Plan.GenericVictimIndices.Contains(generic[0])) stalledSubset++;
            }
            else
            {
                binding++;
                if (!no.Generic.SequenceEqual(on.Generic)) bindingDiffer++;
            }
            if (plan.GenericBudgetLimited > 0) limited++;
        }
        Check(states >= 500 && bound == states, "B1_GENERIC_STEP_NEVER_EXCEEDS_THE_BASAL_AREA_BUDGET_EXCEPT_ONE_STEM_WHEN_NONE_FITS", $"states={states} fallbackStates={fallback}");
        Check(accounting == states && quotaSame == states && sitkaSame == states, "B1_ACCOUNTING_QUOTA_AND_SITKA_COMPONENT_ARE_UNAFFECTED", $"states={states}");
        Check(maximal == states, "B1_BUDGET_IS_FILLED_AS_FAR_AS_THE_QUOTA_ALLOWS", $"states={states} quotaNotMet={limited}");
        Check(inert >= 100 && inertSame == inert && binding >= 100 && bindingDiffer == binding && stalled >= 5 && stalledSubset == stalled, "B2_BUDGET_CHANGES_NOTHING_WHEN_IT_IS_NOT_REACHED_AND_SOMETHING_WHEN_IT_IS", $"inertStates={inert} bindingStates={binding} neverStallStates={stalled}");

        // B3 not a capacity and never stalls: a hugely overstocked stand of large trees is thinned a little every year until the pressure is gone.
        var stand = CanopyStand(Rng(3), 70, 0, GrowingSpaceDemandClass.High);
        double occ0 = MixedDensityModel.Occupancy(stand, 0.16f);
        bool neverStalls = true, withinBudget = true; int years = 0; double occ = occ0, worstShare = 0.0;
        var living = new List<DensityStem>(stand);
        for (int year = 0; year < 400 && occ > 1.05; year++)
        {
            var p = Plan(living, 0.16f, year);
            double ba = living.Sum(StemBa);
            double share = p.Plan.GenericVictimIndices.Sum(i => StemBa(living[i])) / ba;
            worstShare = Math.Max(worstShare, share);
            if (p.Plan.GenericQuota >= 1 && p.Plan.GenericDeaths < 1) neverStalls = false;
            if (share > budgetShare + 1e-9 && p.Plan.GenericVictimIndices.Count != 1) withinBudget = false;
            var dead = new HashSet<int>(p.Plan.VictimIndices);
            living = living.Where((x, i) => !dead.Contains(i)).ToList();
            occ = MixedDensityModel.Occupancy(living, 0.16f);
            years++;
        }
        Check(occ0 > 3.0 && occ <= 1.05 && neverStalls && withinBudget && years > 10 && years < 400, "B3_THE_BUDGET_IS_A_RATE_NOT_A_CAPACITY_AND_NEVER_STALLS_THE_RELIEF", $"occupancy {F(occ0, "0.00")} -> {F(occ, "0.00")} in {years} years, worst yearly share {F(worstShare)}");

        // B4 the defect, reproduced and removed: 32 large stems, 3 recruits. Without the budget the shortfall falls on the canopy.
        double offMax = 0.0, onMax = 0.0, offMean = 0.0; int offBig = 0, trials = 0, onBind = 0;
        for (int year = 0; year < 200; year++)
        {
            var st = CanopyStand(Rng(11 + year % 7), 32, 3);
            double ba = st.Sum(StemBa);
            var a = Plan(st, 0.16f, year, off); var b = Plan(st, 0.16f, year);
            double lossOff = a.Plan.GenericVictimIndices.Sum(i => StemBa(st[i])) / ba, lossOn = b.Plan.GenericVictimIndices.Sum(i => StemBa(st[i])) / ba;
            trials++; offMax = Math.Max(offMax, lossOff); onMax = Math.Max(onMax, lossOn); offMean += lossOff / 200.0;
            if (lossOff > 0.2) offBig++;
            if (b.Plan.GenericBudgetPassedOver > 0) onBind++;
        }
        Check(offMax >= 0.25 && offBig >= 40, "B4_WITHOUT_THE_BUDGET_A_RECRUIT_SHORTAGE_REMOVES_A_QUARTER_OF_THE_CANOPY_BASAL_AREA", $"trials={trials} worstLoss={F(offMax)} meanLoss={F(offMean)} yearsOver20Percent={offBig}");
        Check(onMax <= budgetShare + 1e-9 && onBind >= 100, "B4_WITH_THE_BUDGET_THE_SAME_STANDS_LOSE_AT_MOST_THE_BUDGET", $"worstLoss={F(onMax)} budget={F(budgetShare, "0.000")} yearsBudgetChangedTheVictims={onBind}");

        // B5 recruits available: the budget stays out of the way (the accepted regime). 32 large stems and 40 recruits: the quota is paid by
        // recruits, the victims are exactly those of the unbudgeted planner, and the planner reports that the budget never acted.
        int inertYears = 0, identical5 = 0, richTrials = 0; double richMean = 0.0;
        for (int year = 0; year < 200; year++)
        {
            var st = CanopyStand(Rng(21 + year % 7), 32, 40);
            double ba = st.Sum(StemBa);
            var a = Plan(st, 0.16f, year, off); var b = Plan(st, 0.16f, year);
            richTrials++;
            if (b.Plan.GenericBudgetPassedOver == 0 && b.Plan.GenericBudgetLimited == 0) inertYears++;
            if (a.Generic.SequenceEqual(b.Generic)) identical5++;
            richMean += b.Plan.GenericVictimIndices.Sum(i => StemBa(st[i])) / ba / 200.0;
        }
        Check(inertYears == richTrials && identical5 == richTrials, "B5_WITH_PLENTY_OF_RECRUITS_THE_BUDGET_NEVER_ACTS_AND_THE_VICTIMS_ARE_UNCHANGED", $"trials={richTrials} budgetActed={richTrials - inertYears} meanBasalAreaLoss={F(richMean)}");

        // B6 class neutrality while the budget binds: stems come in equal-size pairs; swapping the two classes inside each pair
        // changes neither the occupied space nor the basal area, so the generic victims must be exactly the same.
        int same6 = 0, binding6 = 0, victims6 = 0;
        for (int year = 0; year < 150; year++)
        {
            var a = new List<DensityStem>(); var b = new List<DensityStem>();
            var rr = Rng(500 + year);
            for (int pair = 0; pair < 16; pair++)
            {
                float dbh = 50f + (float)(rr.NextDouble() * 25), ci = (float)(0.3 + rr.NextDouble() * 2.5);
                a.Add(new DensityStem("P" + (2 * pair).ToString("D3"), GrowingSpaceDemandClass.High, dbh, ci)); a.Add(new DensityStem("P" + (2 * pair + 1).ToString("D3"), GrowingSpaceDemandClass.Intermediate, dbh, ci));
                b.Add(new DensityStem("P" + (2 * pair).ToString("D3"), GrowingSpaceDemandClass.Intermediate, dbh, ci)); b.Add(new DensityStem("P" + (2 * pair + 1).ToString("D3"), GrowingSpaceDemandClass.High, dbh, ci));
            }
            for (int k = 0; k < 4; k++) { a.Add(new DensityStem("Q" + k, GrowingSpaceDemandClass.High, 5f, 20f)); b.Add(new DensityStem("Q" + k, GrowingSpaceDemandClass.Intermediate, 5f, 20f)); }
            var pa = Plan(a, 0.16f, year); var pb = Plan(b, 0.16f, year);
            victims6 += pa.Generic.Count;
            if (pa.Plan.GenericBudgetPassedOver > 0) binding6++;
            if (pa.Generic.SequenceEqual(pb.Generic) && Math.Abs(pa.Plan.GenericBasalAreaShare - pb.Plan.GenericBasalAreaShare) < 1e-12) same6++;
        }
        Check(same6 == 150 && binding6 >= 40 && victims6 >= 300, "B6_THE_BUDGET_IS_CLASS_AND_SPECIES_NEUTRAL_WHILE_IT_BINDS", $"trials=150 budgetBound={binding6} genericVictimsCompared={victims6}");
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
        SeamContinuityQ10();
        ControllerFixturePredictionQ10();
        Shielding();
        GenericComponent();
        CanopyBudget();
        LongRuns();
        Console.WriteLine($"COMPOSITE_DENSITY_{(failures == 0 ? "PASS" : "FAIL")} checks={checks} failures={failures}");
        return failures == 0 ? 0 : 1;
    }
}
