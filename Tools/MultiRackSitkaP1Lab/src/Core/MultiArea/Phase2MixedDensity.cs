namespace SitkaP1.Core;

/// <summary>One living stem as the generic density component sees it: ordinal weight, post-growth DBH and rack Hegyi CI.</summary>
internal readonly record struct Phase2DensityStem(string TreeId, double Weight, bool IsSitka, float DbhCm, double CompetitionIndex);

/// <summary>The generic component's plan for one rack-year (Unity DensityComponentPlan, generic half).</summary>
internal sealed class Phase2GenericPlan
{
    public double Occupancy, NonReferenceShare, GenericResponse, GenericLossFraction, ExpectedCount, Remainder;
    public double StandBasalAreaM2, GenericBasalAreaM2;
    public double? QuotaRoll;
    public int EligibleForGeneric, Quota, BudgetPassedOver;
    public bool SingleStemFallback;
    /// <summary>Indices into the stem list, ordinal TreeId order.</summary>
    public readonly List<int> VictimIndices = new();
}

/// <summary>
/// The exact two-part mixed density route (spec §5, D-061, TRANSFER from Unity MixedDensityModel.cs,
/// MixedDensityModel.Composite.cs PlanComponents and SitkaSubsetDensitySignal.cs at 5b757d9b), evaluated per rack.
/// <list type="bullet">
/// <item>A rack with no living Oak or Beech takes the unchanged Stage 4 step (<see cref="MultiAreaAnnual.StepStage4"/>), bit
/// for bit; nothing here runs and no ADULT-MORT-GENERIC key is drawn.</item>
/// <item>A mixed rack: the Stage 4 Comeau Sitka process runs on the Sitka stems ALONE (stems per hectare of the whole rack,
/// the same ADULT-MORT-{treeId} rolls, the same hazard and RD ≤ 1 boundary); then the generic occupancy component removes a
/// share of the stems still standing, chosen by species-neutral suppression (Ci50 = 5 for every species) within a 0.125
/// basal-area budget.</item>
/// </list>
/// The Sitka arithmetic below is the Stage 4 expression set of AnnualRules.CalculateCore restricted to the Sitka indices;
/// the suppression CI is the rack's own Hegyi index over every living stem of the rack, exactly as Stage 4 (and Unity's
/// DensityStem, which carries each stem's Hegyi index over the whole stand).
/// </summary>
internal static class Phase2MixedDensity
{
    public const string GenericMechanism = "generic-occupancy";

    /// <summary>One rack's Phase 2 adult step: growth for every species, then the routed density mortality.</summary>
    /// <remarks>
    /// <paramref name="scotsPine"/> is null on every woodland without Scots pine / site-moisture state, and the step is then the
    /// unchanged Phase 2 step. With it, a living Scots pine routes the rack through the mixed path (pine is a non-reference
    /// stem of the generic component, never part of the Sitka Comeau subset) and a non-neutral site scales each species' growth
    /// by its moisture growth modifier.
    /// </remarks>
    public static (AnnualReport Report, AnnualState Next, Phase2DensityRecord Record) StepRack(string areaId, AnnualState state,
        int simulationSeed, float areaHa, AnnualTree[] woodland, Func<string, int> pruningLiftsOf, Stage4Options options,
        Func<string, Phase2Species?> broadleafOf, ScotsPineRackContext? scotsPine = null)
    {
        bool hasPine = scotsPine is not null && state.Trees.Any(tree => tree.IsLiving && scotsPine.IsPine(tree.TreeId));
        if (!hasPine && !state.Trees.Any(tree => tree.IsLiving && broadleafOf(tree.TreeId) is not null))
        {
            var pureOptions = scotsPine?.SitkaOptions(options) ?? options;
            var (pureReport, pureNext) = MultiAreaAnnual.StepStage4(state, simulationSeed, areaHa, woodland, pruningLiftsOf, pureOptions);
            return (pureReport, pureNext, PureRecord(areaId, pureReport));
        }
        var report = CalculateMixed(areaId, state, simulationSeed, areaHa, woodland, pruningLiftsOf, options, broadleafOf, scotsPine,
            out var record);
        var next = new AnnualState(report.YearAfter, Array.AsReadOnly(report.Trees.Select(row => row.After).ToArray()));
        return (report, next, record);
    }

    private static Phase2DensityRecord PureRecord(string areaId, AnnualReport report)
    {
        int hazard = report.Deaths.Count(d => d.Mechanism == "hazard");
        int boundary = report.Deaths.Count(d => d.Mechanism == "boundary");
        return new Phase2DensityRecord(areaId, false, report.Before.LivingCount, report.Before.LivingCount, hazard, boundary, 0, 0, 0, 0, 0, 0,
            0, null, 0, 0, false, 0, 0, Array.Empty<string>());
    }

    private static AnnualReport CalculateMixed(string areaId, AnnualState state, int simulationSeed, float areaHa, AnnualTree[] woodland,
        Func<string, int> pruningLiftsOf, Stage4Options options, Func<string, Phase2Species?> broadleafOf, ScotsPineRackContext? scotsPine,
        out Phase2DensityRecord record)
    {
        int year = checked(state.EcologicalYear + 1);
        var before = state.Trees.Where(tree => tree.IsLiving).OrderBy(tree => tree.TreeId, StringComparer.Ordinal).ToArray();
        var species = before.Select(tree => broadleafOf(tree.TreeId)).ToArray();
        var pine = before.Select(tree => scotsPine?.IsPine(tree.TreeId) ?? false).ToArray();
        var competition = before.Select(tree => AnnualRules.Competition(tree, before)).ToArray();
        // Scots pine reads its rack's start-of-year stocking, basal area and top height (the Part 1 GROWFOR request).
        var rackStand = scotsPine is null ? null : Stand(before, areaHa);
        double topHeight = scotsPine is null || before.Length == 0 ? 0.0 : before.Max(tree => (double)tree.HeightM);
        var grown = before.Select((tree, i) =>
        {
            float woodlandCi = AnnualRules.Competition(tree, woodland).Ci;
            if (pine[i])
                return ScotsPineAdult.Grow(tree, woodlandCi, rackStand!, topHeight, scotsPine!.Field, options, pruningLiftsOf(tree.TreeId),
                    scotsPine.Tally);
            float site = scotsPine is null ? options.SiteAt(tree)
                : scotsPine.SiteFor(options, tree, species[i]?.SpeciesId ?? Phase2Species.SitkaId);
            return species[i] is { } broadleaf
                ? Phase2Adult.Grow(tree, broadleaf, woodlandCi, site, pruningLiftsOf(tree.TreeId))
                : AnnualRules.GrowStage4(tree, woodlandCi, site, options.GrowthAgePhases, pruningLiftsOf(tree.TreeId));
        }).ToArray();
        var postGrowth = Stand(grown, areaHa);

        // 1. SITKA COMPONENT: the Stage 4 Comeau process on the Sitka stems alone (indices in ordinal TreeId order).
        var sitka = Enumerable.Range(0, before.Length).Where(i => species[i] is null && !pine[i]).ToArray();
        var audit = new AnnualTreeAudit[before.Length];
        var deaths = new List<AnnualDeath>();
        var after = (AnnualTree[])grown.Clone();
        double pressure = SitkaGrowthModel.DensityPressure(Stand(sitka.Select(i => grown[i]), areaHa).RelativeDensity);
        double? meanVulnerability = null;
        var suppression = new Dictionary<int, double>();
        if (pressure > 0.0 && sitka.Length > 0)
        {
            double sum = 0.0;
            foreach (int i in sitka)
            {
                suppression[i] = 1.0 - 1.0 / (1.0 + competition[i].Ci / SpeciesProfile.Sitka.Ci50);
                sum += SitkaGrowthModel.Vulnerability(suppression[i]);
            }
            meanVulnerability = sum / sitka.Length;
        }
        var survivors = new List<int>();
        int sitkaHazard = 0;
        for (int i = 0; i < before.Length; i++)
        {
            var (ci, neighbours) = competition[i];
            if (species[i] is not null || pine[i])
            {
                audit[i] = new AnnualTreeAudit(before[i], grown[i], neighbours, ci, null, null, null);
                continue;
            }
            double? s = suppression.TryGetValue(i, out var value) ? value : null;
            double? probability = s is double sv ? SitkaGrowthModel.AnnualDeathProbability(pressure, sv, meanVulnerability!.Value) : null;
            float? roll = null;
            if (probability > 0.0)
                roll = SimulationRandom.Roll(DerivedManifest.RngModelVersion, RngSalts.AdultMortality + before[i].TreeId, year, simulationSeed);
            if (roll < probability)
            {
                var death = new AnnualDeath(grown[i].TreeId, year, LifeStates.DensitySuppression, "hazard");
                deaths.Add(death);
                after[i] = LifeStates.ApplyBiologicalDeath(grown[i], death);
                sitkaHazard++;
            }
            else
                survivors.Add(i);
            audit[i] = new AnnualTreeAudit(before[i], after[i], neighbours, ci, s, probability, roll);
        }
        int sitkaBoundary = 0;
        if (suppression.Count > 0)
            sitkaBoundary = ApplySitkaBoundary(audit, after, survivors, areaHa, year, deaths, suppression);

        // 2. GENERIC COMPONENT on every living stem (post-growth DBH, rack CI); the Sitka victims are not eligible.
        var stems = before.Select((tree, i) => new Phase2DensityStem(tree.TreeId,
            species[i]?.GrowingSpaceWeight ?? (pine[i] ? ScotsPineLive.GrowingSpaceWeight : Phase2Constants.SitkaGrowingSpaceWeight),
            species[i] is null && !pine[i], grown[i].DbhCm, competition[i].Ci)).ToArray();
        var taken = new HashSet<int>(Enumerable.Range(0, before.Length).Where(i => !after[i].IsLiving));
        var plan = PlanGeneric(stems, areaHa, taken,
            key => Phase2Random.Roll(key == QuotaKeyToken ? Phase2Random.GenericQuotaKey(areaId) : Phase2Random.GenericTreeKey(key),
                year, simulationSeed));
        foreach (int i in plan.VictimIndices)
        {
            var death = new AnnualDeath(grown[i].TreeId, year, LifeStates.DensitySuppression, GenericMechanism);
            deaths.Add(death);
            after[i] = LifeStates.ApplyBiologicalDeath(grown[i], death);
            audit[i] = audit[i] with { After = after[i] };
        }

        record = new Phase2DensityRecord(areaId, true, before.Length, sitka.Length, sitkaHazard, sitkaBoundary, plan.Occupancy,
            plan.GenericResponse, plan.NonReferenceShare, plan.EligibleForGeneric, plan.ExpectedCount, plan.Quota, plan.Remainder,
            plan.QuotaRoll, plan.VictimIndices.Count, plan.BudgetPassedOver, plan.SingleStemFallback, plan.StandBasalAreaM2,
            plan.GenericBasalAreaM2, Array.AsReadOnly(plan.VictimIndices.Select(i => before[i].TreeId).ToArray()));
        return new AnnualReport(state.EcologicalYear, year, Stand(before, areaHa), postGrowth,
            Stand(after.Where(tree => tree.IsLiving), areaHa), pressure, meanVulnerability,
            Array.AsReadOnly(audit.Concat(state.Trees.Where(tree => !tree.IsLiving)
                .Select(tree => new AnnualTreeAudit(tree, tree with { Death = tree.Death }, 0, 0f, null, null, null)))
                .OrderBy(row => row.Before.TreeId, StringComparer.Ordinal).ToArray()), deaths.AsReadOnly());
    }

    // The Stage 4 RD ≤ 1.0 boundary (AnnualRules.ApplyBoundary) restricted to the Sitka survivors: most suppressed first, ties
    // by ordinal ID; RD from the Sitka survivors over the whole rack area.
    private static int ApplySitkaBoundary(AnnualTreeAudit[] audit, AnnualTree[] after, List<int> survivors, float areaHa, int year,
        List<AnnualDeath> deaths, Dictionary<int, double> suppression)
    {
        int count = survivors.Count;
        if (count <= 1) return 0;
        double squaredDbhSum = 0.0;
        foreach (int i in survivors)
            squaredDbhSum += (double)audit[i].After.DbhCm * audit[i].After.DbhCm;
        double RelativeDensity() => SitkaGrowthModel.RelativeDensity(count / areaHa, Math.Sqrt(squaredDbhSum / count));

        if (RelativeDensity() <= SitkaGrowthModel.MaximumRelativeDensity) return 0;
        int removed = 0;
        foreach (int i in survivors.OrderByDescending(i => suppression[i]).ThenBy(i => i))
        {
            if (count <= 1 || RelativeDensity() <= SitkaGrowthModel.MaximumRelativeDensity) break;
            var tree = audit[i].After;
            count--;
            squaredDbhSum -= (double)tree.DbhCm * tree.DbhCm;
            var death = new AnnualDeath(tree.TreeId, year, LifeStates.DensitySuppression, "boundary");
            deaths.Add(death);
            after[i] = LifeStates.ApplyBiologicalDeath(tree, death);
            audit[i] = audit[i] with { After = after[i] };
            removed++;
        }
        return removed;
    }

    /// <summary>The roll-function argument that names the quota draw (Unity GenericQuotaRollKey).</summary>
    public const string QuotaKeyToken = "~generic-quota";

    /// <summary>
    /// The generic component (Unity PlanComponents, generic half), over <paramref name="stems"/> in ordinal TreeId order:
    ///   space_i = w · (π/4) · DBH_m²; O = (Σ space / area_ha) / 55; G(O) = 0.5 · (1 − exp(−1.60 · max(0, O − 1)²));
    ///   share = Σ_broadleaf space / Σ space; expected = G · share · eligible; quota = floor(expected + 1e-10) + [roll &lt; remainder];
    ///   victims by an exponential race on species-neutral weights S⁴ (S = CI/(CI + 5)), paid within 0.125 × stand basal area,
    ///   passing over stems that do not fit; if none fits, the single highest-priority stem.
    /// <paramref name="roll"/> receives <see cref="QuotaKeyToken"/> for the quota draw and a TreeId for each stem's draw.
    /// </summary>
    public static Phase2GenericPlan PlanGeneric(IReadOnlyList<Phase2DensityStem> stems, float areaHa, ISet<int> sitkaVictims,
        Func<string, double> roll)
    {
        var plan = new Phase2GenericPlan();
        if (stems.Count == 0 || areaHa <= 0f) return plan;
        double occupied = 0.0, nonReference = 0.0;
        for (int i = 0; i < stems.Count; i++)
        {
            double space = OccupiedSpace(stems[i]);
            occupied += space;
            if (!stems[i].IsSitka) nonReference += space;
            plan.StandBasalAreaM2 += BasalArea(stems[i]);
        }
        plan.Occupancy = occupied / areaHa / Phase2Constants.OccupancyNormalization;
        plan.NonReferenceShare = occupied > 0.0 ? nonReference / occupied : 0.0;
        plan.GenericResponse = GenericLoss(plan.Occupancy);
        plan.GenericLossFraction = plan.GenericResponse * plan.NonReferenceShare;
        plan.EligibleForGeneric = stems.Count - sitkaVictims.Count;

        plan.ExpectedCount = plan.GenericLossFraction * plan.EligibleForGeneric;
        int quota = (int)Math.Floor(plan.ExpectedCount + 1e-10);
        plan.Remainder = Math.Max(0.0, plan.ExpectedCount - quota);
        if (plan.Remainder > 0.0)
        {
            plan.QuotaRoll = roll(QuotaKeyToken);
            if (plan.QuotaRoll < plan.Remainder) quota++;
        }
        plan.Quota = quota = Math.Min(plan.EligibleForGeneric, quota);
        if (quota <= 0) return plan;

        var eligible = new List<int>(plan.EligibleForGeneric);
        double totalVulnerability = 0.0;
        for (int i = 0; i < stems.Count; i++)
        {
            if (sitkaVictims.Contains(i)) continue;
            eligible.Add(i);
            totalVulnerability += Vulnerability(Suppression(stems[i].CompetitionIndex));
        }
        var priority = new Dictionary<int, double>(eligible.Count);
        foreach (int i in eligible)
        {
            double weight = totalVulnerability > 0.0 ? Vulnerability(Suppression(stems[i].CompetitionIndex)) : 1.0;
            double u = Math.Max(1e-15, Math.Min(1.0 - 1e-15, roll(stems[i].TreeId)));
            priority[i] = weight > 0.0 ? -Math.Log(u) / weight : double.PositiveInfinity;
        }
        eligible.Sort((a, b) =>
        {
            int byPriority = priority[a].CompareTo(priority[b]);
            return byPriority != 0 ? byPriority : string.CompareOrdinal(stems[a].TreeId, stems[b].TreeId);
        });
        double budget = Phase2Constants.GenericAnnualBasalAreaBudgetShare * plan.StandBasalAreaM2;
        double spent = 0.0;
        foreach (int i in eligible)
        {
            if (plan.VictimIndices.Count >= quota) break;
            double basalArea = BasalArea(stems[i]);
            if (spent + basalArea > budget + 1e-12) { plan.BudgetPassedOver++; continue; }
            plan.VictimIndices.Add(i);
            spent += basalArea;
        }
        if (plan.VictimIndices.Count == 0)
        {
            plan.VictimIndices.Add(eligible[0]);
            spent = BasalArea(stems[eligible[0]]);
            plan.SingleStemFallback = true;
        }
        plan.GenericBasalAreaM2 = spent;
        plan.VictimIndices.Sort((a, b) => string.CompareOrdinal(stems[a].TreeId, stems[b].TreeId));
        return plan;
    }

    /// <summary>G(O) = 0.5 · (1 − exp(−1.60 · excess²)), excess = max(0, O − 1); 0 at and below the onset.</summary>
    public static double GenericLoss(double occupancy)
    {
        double excess = Math.Max(0.0, occupancy - Phase2Constants.OnsetOccupancy);
        if (excess <= 0.0) return 0.0;
        return Phase2Constants.GenericMaximumAnnualLoss *
            (1.0 - Math.Exp(-Phase2Constants.GenericResponseStrength * Math.Pow(excess, Phase2Constants.GenericResponseExponent)));
    }

    /// <summary>Species-neutral suppression S = 1 − 1/(1 + max(0, CI)/5), clamped to [0, 1] (Ci50 = 5 for every species).</summary>
    public static double Suppression(double competitionIndex)
    {
        double s = 1.0 - 1.0 / (1.0 + Math.Max(0.0, competitionIndex) / Phase2Constants.NeutralHalfSaturationCi);
        return Math.Max(0.0, Math.Min(1.0, s));
    }

    public static double Vulnerability(double suppression) =>
        Math.Pow(Math.Max(0.0, Math.Min(1.0, suppression)), Phase2Constants.VulnerabilityExponent);

    /// <summary>w · (π/4) · DBH_m², the class-weighted occupied cross-section (m²).</summary>
    public static double OccupiedSpace(Phase2DensityStem stem)
    {
        double dbhM = stem.DbhCm / 100.0;
        return stem.Weight * (Math.PI / 4.0) * dbhM * dbhM;
    }

    /// <summary>Plain basal area (π/4) · DBH_m², no weight.</summary>
    public static double BasalArea(Phase2DensityStem stem)
    {
        double dbhM = stem.DbhCm / 100.0;
        return (Math.PI / 4.0) * dbhM * dbhM;
    }

    // AnnualRules.Stand, unchanged (it is private there): counts, stems/ha, BA/ha, Dq and the Sitka RD of a stem set.
    private static AnnualStand Stand(IEnumerable<AnnualTree> living, float areaHa)
    {
        int count = 0;
        double squaredDbhSum = 0.0;
        foreach (var tree in living)
        {
            count++;
            squaredDbhSum += (double)tree.DbhCm * tree.DbhCm;
        }
        if (count == 0) return new AnnualStand(0, 0, 0, 0, 0);
        double stemsPerHa = count / areaHa;
        double dq = Math.Sqrt(squaredDbhSum / count);
        double basalAreaPerHa = squaredDbhSum * (Math.PI / 40000.0) / areaHa;
        return new AnnualStand(count, stemsPerHa, basalAreaPerHa, dq, SitkaGrowthModel.RelativeDensity(stemsPerHa, dq));
    }
}
