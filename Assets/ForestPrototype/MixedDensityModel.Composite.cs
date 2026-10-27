using System;
using System.Collections.Generic;

// The shape numbers (and the bound) of the GENERIC occupancy response, plus the basal-area budget of one generic step.
// Passed explicitly so a check or a calibration search can change them; production code always uses Default.
public readonly struct GenericResponseCalibration
{
    // k: how quickly the loss rises with occupancy beyond the onset.
    public readonly double Strength;
    // Upper bound of the response, approached only as occupancy grows without limit.
    public readonly double MaximumLoss;
    // m: 2 starts flat at the onset (zero slope), 1 starts with a finite slope. Both are continuous, monotonic and bounded.
    public readonly double Exponent;
    // Largest share of the stand's basal area the generic component may remove in one annual step (one stem is still taken when
    // none fits). 1.0 means no budget, since a stem share cannot exceed it. The production default is
    // MixedDensityModel.GenericAnnualBasalAreaBudgetShare.
    public readonly double BasalAreaBudgetShare;

    public GenericResponseCalibration(double strength, double maximumLoss, double exponent)
        : this(strength, maximumLoss, exponent, MixedDensityModel.GenericAnnualBasalAreaBudgetShare)
    {
    }

    public GenericResponseCalibration(double strength, double maximumLoss, double exponent, double basalAreaBudgetShare)
    {
        if (!(strength > 0.0)) throw new ArgumentOutOfRangeException(nameof(strength), strength, "strength must be positive");
        if (!(maximumLoss > 0.0 && maximumLoss < 1.0)) throw new ArgumentOutOfRangeException(nameof(maximumLoss), maximumLoss, "maximum loss must be in (0, 1)");
        if (!(exponent >= 1.0)) throw new ArgumentOutOfRangeException(nameof(exponent), exponent, "exponent must be at least 1");
        if (!(basalAreaBudgetShare > 0.0 && basalAreaBudgetShare <= 1.0)) throw new ArgumentOutOfRangeException(nameof(basalAreaBudgetShare), basalAreaBudgetShare, "basal-area budget must be in (0, 1]");
        Strength = strength;
        MaximumLoss = maximumLoss;
        Exponent = exponent;
        BasalAreaBudgetShare = basalAreaBudgetShare;
    }

    public static GenericResponseCalibration Default =>
        new GenericResponseCalibration(MixedDensityModel.GenericResponseStrength, MixedDensityModel.GenericMaximumAnnualLoss, MixedDensityModel.GenericResponseExponent,
            MixedDensityModel.GenericAnnualBasalAreaBudgetShare);
}

// Outcome of one annual mixed-stand step, kept per component so each can be inspected on its own.
public sealed class DensityComponentPlan : DensityMortalityPlan
{
    // Share of the occupied space that belongs to non-Reference stems (the generic component scales with it, so it
    // vanishes continuously as the broadleaves vanish).
    public double NonReferenceShare;
    // Generic response at this occupancy, before the share: a fraction of the stems that are still standing after the
    // Sitka component.
    public double GenericResponse;
    // That response times the share: the fraction of the remaining stems the generic component is expected to remove.
    public double GenericLossFraction;
    // Stems left for the generic component to choose from: all stems minus the Sitka component's victims.
    public int EligibleForGeneric;
    public int SitkaHazardDeaths;
    public int SitkaBoundaryDeaths;
    // Whole-stem quota the generic component drew for the year, before the basal-area budget.
    public int GenericQuota;
    public int GenericDeaths;
    // Stems of the quota that were not removed because the basal-area budget was spent (GenericQuota - GenericDeaths).
    public int GenericBudgetLimited;
    // Stems passed over because they did not fit the budget while the quota was still open. Above zero means the budget changed
    // this year's victims: without it, those stems would have been among them.
    public int GenericBudgetPassedOver;
    // Basal area removed by the generic component, as a share of the stand's basal area before the step.
    public double GenericBasalAreaShare;
    // Indices into the stem list, ordinal tree-id order. VictimIndices is their union; the two never overlap.
    public readonly List<int> SitkaVictimIndices = new List<int>();
    public readonly List<int> GenericVictimIndices = new List<int>();
}

public static partial class MixedDensityModel
{
    // [C] GENERIC occupancy response: loss = maximum x (1 - exp(-strength x excess^exponent)), excess = occupancy - 1.
    // Zero at and below the onset, continuous, monotonic, bounded; the same for every species. It is not a broadleaf
    // relative density, not a maximum density and not a basal-area limit; occupancy is the ordinal growing-space
    // abstraction in MixedDensityModel.cs.
    public const double GenericResponseStrength = 1.60;
    public const double GenericMaximumAnnualLoss = 0.50;
    public const double GenericResponseExponent = 2.0;

    // [C] BASAL-AREA BUDGET of one generic step. The response above is a fraction of STEMS. Stems under about 20 cm carry almost
    // all of the allocation weight and almost none of the basal area, so the quota is normally paid by them at little cost to the
    // canopy. When fewer of them exist than the quota (recruits arrive in a cohort and are eaten, a year with no recruits), the
    // rest of the quota falls on canopy stems, which all have a tiny and nearly equal weight: a shortfall of a few stems then
    // removes a quarter or more of the stand's basal area in one year (Docs/Verification/ThreeSpeciesProgramme/CanopyPulseRootCause.md).
    // The budget bounds that: stems are taken in the usual priority order and a stem that would take the generic step above this
    // share of the stand's basal area is passed over. Only if no stem fits at all (a stand of fewer than 1/share stems) is the
    // highest-priority stem taken, so a sparse stand of a few large trees is never frozen. The budget is memoryless: occupancy
    // is recomputed each year, so a stand that is still crowded keeps asking and is thinned over several years instead of in one.
    // [C] calibration, not a measurement and not a species, class or maximum-density value. Reduced-model sweep (16 seeds x 5
    // recruitment regimes x 8 pure stands; CanopyPulseRootCause.md): 0.125 to 0.15 stop the pulses and leave the accepted
    // even-recruitment dynamics inside seed noise; 0.12 is borderline; 0.11 and below slow the flattening of ~20-stem Legacy40
    // Oak stands; 0.175 and above no longer stop 15 % pulses. 0.125 is the smallest tested value with no measurable effect on
    // the accepted regime. Read as stems: a stand of eight or more similar canopy stems can always lose one in a year, sixteen or
    // more can lose two.
    public const double GenericAnnualBasalAreaBudgetShare = 0.125;

    // Key of the one extra draw that turns a fractional expected count into a whole number of stems.
    public const string GenericQuotaRollKey = "~generic-quota";

    public static double GenericLoss(double occupancy)
    {
        return GenericLoss(occupancy, GenericResponseCalibration.Default);
    }

    public static double GenericLoss(double occupancy, GenericResponseCalibration calibration)
    {
        double excess = Math.Max(0.0, occupancy - OnsetOccupancy);
        if (excess <= 0.0) return 0.0;
        return calibration.MaximumLoss * (1.0 - Math.Exp(-calibration.Strength * Math.Pow(excess, calibration.Exponent)));
    }

    public static DensityComponentPlan PlanComponents(IReadOnlyList<DensityStem> stems, float areaHectares,
        IReadOnlyList<int> sitkaHazardVictims, IReadOnlyList<int> sitkaBoundaryVictims, Func<string, double> genericRoll)
    {
        return PlanComponents(stems, areaHectares, sitkaHazardVictims, sitkaBoundaryVictims, genericRoll, GenericResponseCalibration.Default);
    }

    // COMPONENT-FAITHFUL mortality for a stand that has at least one non-Reference stem.
    //
    //   SITKA COMPONENT  The Sitka process was already resolved on the Sitka stems alone (SitkaSubsetDensitySignal.Resolve);
    //                    its victims arrive here as stem indices (this file never sees Sitka arithmetic) and are taken as
    //                    given. They are not reallocated.
    //   GENERIC          Total occupancy (class-weighted cross-section, MixedDensityModel.cs) goes through the generic
    //                    response, times the non-Reference share of the occupied space. That fraction of the stems still
    //                    standing after the Sitka component is removed, chosen among those stems only, by competitive
    //                    position alone (species-neutral Hegyi suppression, one common half saturation and exponent).
    //                    No species, class or Ci50 enters the choice. The whole-stem quota is paid within a basal-area budget
    //                    (GenericAnnualBasalAreaBudgetShare): see that constant.
    //   FINAL VICTIMS    The two sets are disjoint by construction (the generic draw cannot pick a stem the Sitka
    //                    component already took), so no stem dies twice and the total is Sitka deaths plus generic deaths.
    //
    // Equivalent to the earlier combined quota 1 - (1 - sitka)(1 - generic) in COUNT (sitkaDeaths + generic x remaining
    // stems) unless the basal-area budget binds; what changes is WHO pays the Sitka share. With no broadleaves the share is
    // zero and nothing here runs.
    public static DensityComponentPlan PlanComponents(IReadOnlyList<DensityStem> stems, float areaHectares,
        IReadOnlyList<int> sitkaHazardVictims, IReadOnlyList<int> sitkaBoundaryVictims, Func<string, double> genericRoll,
        GenericResponseCalibration calibration)
    {
        if (sitkaHazardVictims == null) throw new ArgumentNullException(nameof(sitkaHazardVictims));
        if (sitkaBoundaryVictims == null) throw new ArgumentNullException(nameof(sitkaBoundaryVictims));
        var plan = new DensityComponentPlan();
        if (stems.Count == 0 || areaHectares <= 0f) return plan;

        var taken = new bool[stems.Count];
        foreach (int i in sitkaHazardVictims) MarkTaken(stems, taken, i);
        foreach (int i in sitkaBoundaryVictims) MarkTaken(stems, taken, i);

        int[] order = CanonicalOrder(stems);
        double occupied = 0.0, nonReference = 0.0, squares = 0.0;
        foreach (int i in order)
        {
            double space = StemOccupiedSpace(stems[i]);
            occupied += space;
            if (stems[i].Demand != GrowingSpaceDemandClass.Reference) nonReference += space;
            squares += (double)stems[i].DbhCm * stems[i].DbhCm;
        }
        plan.StemsPerHectare = stems.Count / areaHectares;
        plan.QuadraticMeanDbhCm = Math.Sqrt(squares / stems.Count);
        plan.Occupancy = OccupancyOf(occupied, areaHectares, OccupancyCalibration.Default);
        plan.NonReferenceShare = occupied > 0.0 ? nonReference / occupied : 0.0;
        plan.GenericResponse = GenericLoss(plan.Occupancy, calibration);
        plan.GenericLossFraction = plan.GenericResponse * plan.NonReferenceShare;
        plan.SitkaHazardDeaths = sitkaHazardVictims.Count;
        plan.SitkaBoundaryDeaths = sitkaBoundaryVictims.Count;
        plan.EligibleForGeneric = stems.Count - plan.SitkaHazardDeaths - plan.SitkaBoundaryDeaths;

        // Whole stems: floor of the expected count plus one keyed Bernoulli for the fraction, so a small expected count is
        // neither always rounded away nor always rounded up.
        double expectedCount = plan.GenericLossFraction * plan.EligibleForGeneric;
        int quota = (int)Math.Floor(expectedCount + 1e-10);
        double remainder = Math.Max(0.0, expectedCount - quota);
        if (remainder > 0.0 && genericRoll(GenericQuotaRollKey) < remainder) quota++;
        quota = Math.Min(plan.EligibleForGeneric, quota);

        if (quota > 0)
        {
            var eligible = new List<int>(plan.EligibleForGeneric);
            double totalVulnerability = 0.0;
            foreach (int i in order)
            {
                if (taken[i]) continue;
                eligible.Add(i);
                totalVulnerability += Vulnerability(Suppression(stems[i].CompetitionIndex));
            }
            // Weighted draw without replacement (exponential race): the stem with the smallest -ln(u)/weight is taken
            // first. Weight is competitive vulnerability alone; if nothing is vulnerable every eligible stem weighs 1.
            var priority = new Dictionary<int, double>(eligible.Count);
            foreach (int i in eligible)
            {
                double weight = totalVulnerability > 0.0 ? Vulnerability(Suppression(stems[i].CompetitionIndex)) : 1.0;
                double u = Math.Max(1e-15, Math.Min(1.0 - 1e-15, genericRoll(stems[i].TreeId)));
                priority[i] = weight > 0.0 ? -Math.Log(u) / weight : double.PositiveInfinity;
            }
            eligible.Sort((a, b) =>
            {
                int byPriority = priority[a].CompareTo(priority[b]);
                return byPriority != 0 ? byPriority : string.CompareOrdinal(stems[a].TreeId, stems[b].TreeId);
            });
            // Walk the priority order. Take a stem unless it would take the step above the basal-area budget. Passing a large
            // stem over lets a later, smaller one still be taken, so the quota is met by whatever fits. While the budget is not
            // reached this is exactly "the first quota stems in priority order". If NO eligible stem fits (every stem alone is
            // more than the budget, a stand of fewer than 1/budget stems), the highest-priority stem is taken so that the
            // generic component never stalls on a sparse stand of a few large trees.
            double standBasalArea = 0.0;
            foreach (int i in order) standBasalArea += StemBasalArea(stems[i]);
            double budget = calibration.BasalAreaBudgetShare * standBasalArea;
            double spent = 0.0;
            foreach (int i in eligible)
            {
                if (plan.GenericVictimIndices.Count >= quota) break;
                double basalArea = StemBasalArea(stems[i]);
                if (spent + basalArea > budget + 1e-12) { plan.GenericBudgetPassedOver++; continue; }
                plan.GenericVictimIndices.Add(i);
                spent += basalArea;
            }
            if (plan.GenericVictimIndices.Count == 0)
            {
                plan.GenericVictimIndices.Add(eligible[0]);
                spent = StemBasalArea(stems[eligible[0]]);
            }
            plan.GenericBasalAreaShare = standBasalArea > 0.0 ? spent / standBasalArea : 0.0;
        }
        plan.GenericQuota = quota;
        plan.GenericDeaths = plan.GenericVictimIndices.Count;
        plan.GenericBudgetLimited = plan.GenericQuota - plan.GenericDeaths;

        plan.SitkaVictimIndices.AddRange(sitkaHazardVictims);
        plan.SitkaVictimIndices.AddRange(sitkaBoundaryVictims);
        Comparison<int> byTreeId = (a, b) => string.CompareOrdinal(stems[a].TreeId, stems[b].TreeId);
        plan.SitkaVictimIndices.Sort(byTreeId);
        plan.GenericVictimIndices.Sort(byTreeId);
        plan.VictimIndices.AddRange(plan.SitkaVictimIndices);
        plan.VictimIndices.AddRange(plan.GenericVictimIndices);
        plan.VictimIndices.Sort(byTreeId);
        plan.HazardDeaths = plan.SitkaHazardDeaths + plan.GenericDeaths;
        plan.ExpectedAnnualLoss = (plan.SitkaHazardDeaths + plan.SitkaBoundaryDeaths + plan.GenericLossFraction * plan.EligibleForGeneric) / stems.Count;
        plan.OvercrowdingPressure = plan.ExpectedAnnualLoss;
        return plan;
    }

    // Plain geometry: cross-section at breast height in m2. No class weight, no species.
    private static double StemBasalArea(DensityStem stem)
    {
        double dbhM = stem.DbhCm / 100.0;
        return (Math.PI / 4.0) * dbhM * dbhM;
    }

    private static void MarkTaken(IReadOnlyList<DensityStem> stems, bool[] taken, int index)
    {
        if (index < 0 || index >= stems.Count) throw new ArgumentOutOfRangeException(nameof(index), index, "Sitka victim index outside the stem list");
        if (stems[index].Demand != GrowingSpaceDemandClass.Reference) throw new ArgumentException("the Sitka component can only take Reference stems");
        if (taken[index]) throw new ArgumentException("a stem appears twice among the Sitka victims");
        taken[index] = true;
    }
}
