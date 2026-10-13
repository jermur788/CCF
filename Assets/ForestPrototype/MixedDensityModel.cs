using System;
using System.Collections.Generic;

// Adult density for growth model 2 (D-059, D-061). Two routes handle the annual density step:
//
//   LegacySitkaExact       Every living adult is Reference class (Sitka). ForestEcologyController keeps running the
//                          original growth-model-1 code: the British Sitka size-density line, its hazard and its
//                          boundary (SitkaGrowthModel). Nothing in this file runs, so pure Sitka is model 1 exactly.
//   SpeciesAwareOccupancy  At least one living adult is not Reference class (Oak, Beech). Everything below applies.
//
// This file deliberately uses NOTHING from SitkaGrowthModel: the offline check compiles it without that file. The
// Sitka size-density line, the Sitka hazard and the Sitka maximum line are evidence for Sitka only and are not
// borrowed for other species.
//
// OCCUPANCY - WHEN a mixed stand is overcrowded. A simulation abstraction of how much of the stand's growing space
// is taken up. It is NOT an SDI, NOT Reineke's rule, NOT the British Sitka relative density and NOT a biological
// maximum-density law, and it claims no species-specific equation:
//
//   occupancy = sum over living stems of (demand weight x stem basal area) / stand area / capacity
//
//   - Stem basal area (pi/4 x DBH^2) is plain geometry. It is a size footprint, not a fitted size-density exponent.
//   - The demand weight is the stem's ordinal growing-space-demand class (D-061). A High-demand stem takes more of
//     the same space than a Reference stem of the same size. Species enter ONLY through this class.
//   - The sum is plain addition. Species do not interact: no DEC, no DMC, no overyielding, no mixture bonus.
//
// PRESSURE AND LOSS - HOW MUCH mortality. Pressure rises from 0 at the onset fraction of capacity to 1 at capacity and
// keeps rising above it. The expected annual loss fraction of the stand is strength x pressure^2.
//
// VICTIMS - WHICH stems die. Only competitive position decides, never species or class: the existing Hegyi index
// (species-neutral: neighbour diameters and distances only) is turned into suppression with ONE common half
// saturation. Risk is spread over stems in proportion to suppression^exponent and the boundary removes the most
// suppressed first (ties by ordinal tree id). A species' own Ci50 is deliberately not used here: it would make
// Oak/Beech (Ci50 3) read as more suppressed than Sitka (Ci50 5) at equal competition and so be removed first.
// Shade tolerance belongs to a later, deliberate species-behaviour step, not to this one.
//
// Status of every number (labels: [ORD] ordinal rank, [C] simulation/gameplay calibration, not a measurement):
//   - class weights 1.0 / 1.5 / 2.0: [ORD] rank, [C] magnitude, provisional (D-061). Not measured Oak or Beech data.
//   - capacity 90 m2/ha of Reference-equivalent basal area: [C]. There is no evidence for any absolute capacity for
//     Oak or Beech. 90 was chosen only so a stand of Reference-weight stems lands close to the accepted Sitka line
//     (measured: occupancy is 5% below to 10% above Sitka relative density for Dq 8-80 cm at equal stems/ha), so
//     adding the first Oak to a Sitka stand does not make the Sitka part suddenly immune or suddenly doomed. It is a
//     continuity choice, not the Sitka equation and not a claim about any species.
//   - onset 0.6, strength 0.08, loss cap 0.5, vulnerability exponent 4, neutral Ci 5: [C]. They give the same
//     response shape as growth model 1 so Reference-weight stems behave alike either side of the route switch. They
//     were not fitted to Oak or Beech and no evidence exists to do so. Recalibrate them here, in one place.
//
// No UnityEngine types: the model runs and is verified outside the Editor.

// Which implementation handled an annual density step. Diagnostic only; never saved.
public enum DensityRoute
{
    // The stand was empty or had no area, so no route ran.
    None = 0,
    // Original growth-model-1 Sitka implementation, unchanged.
    LegacySitkaExact = 1,
    // The occupancy model in MixedDensityModel.
    SpeciesAwareOccupancy = 2
}

// Ordinal growing-space demand of one stem relative to the Sitka reference. Serialized on TreeSpeciesDefinition
// (stored as an int in assets, so the numeric values must not change).
public enum GrowingSpaceDemandClass
{
    // The Sitka reference. A stand of only Reference stems keeps the original Sitka route.
    Reference = 0,
    // Takes more growing space per stem than the reference.
    Intermediate = 1,
    // Takes the most growing space per stem of the three classes.
    High = 2
}

// One living adult stem as the density model sees it. Deliberately has no species id: species can only matter
// through Demand.
public readonly struct DensityStem
{
    public readonly string TreeId;
    public readonly GrowingSpaceDemandClass Demand;
    public readonly float DbhCm;
    // Existing Hegyi competition index of the tree. Species-neutral by construction.
    public readonly double CompetitionIndex;

    public DensityStem(string treeId, GrowingSpaceDemandClass demand, float dbhCm, double competitionIndex)
    {
        TreeId = treeId ?? "";
        Demand = demand;
        DbhCm = dbhCm;
        CompetitionIndex = competitionIndex;
    }
}

// Outcome of one annual step. Victims are indices into the stem list the plan was made from, in the order they die:
// hazard victims in ordinal tree-id order, then boundary victims most suppressed first.
public sealed class DensityMortalityPlan
{
    public double StemsPerHectare;          // all living stems (descriptive)
    public double QuadraticMeanDbhCm;       // all living stems pooled (descriptive)
    public double Occupancy;                // before any death; 1.0 = capacity
    public double OccupancyAfter;           // after the planned deaths
    public double OvercrowdingPressure;
    public int HazardDeaths;
    public int BoundaryDeaths;
    public readonly List<int> VictimIndices = new List<int>();
}

public static class MixedDensityModel
{
    // [ORD/C] class weights. Coarse, deliberately round ordinal steps, not fitted values.
    public const double ReferenceWeight = 1.0;
    public const double IntermediateWeight = 1.5;
    public const double HighWeight = 2.0;

    // [C] occupancy capacity in m2/ha of Reference-equivalent basal area (see the header: a continuity choice).
    public const double OccupancyCapacityM2PerHa = 90.0;
    // [C] fraction of capacity at which overcrowding pressure starts.
    public const double OvercrowdingOnset = 0.6;
    // [C] expected annual loss fraction of the stand at pressure 1 (strength x pressure^2).
    public const double OvercrowdingStrength = 0.08;
    // [C] numerical bound on one stem's annual loss probability.
    public const double MaximumAnnualLossProbability = 0.5;
    // [C] spreads risk towards suppressed stems (suppression^exponent), as in growth model 1.
    public const double VulnerabilityExponent = 4.0;
    // [C] one Hegyi half-saturation for every species, so suppression does not depend on species.
    public const double NeutralHalfSaturationCi = 5.0;

    public static double Weight(GrowingSpaceDemandClass demand)
    {
        switch (demand)
        {
            case GrowingSpaceDemandClass.Reference: return ReferenceWeight;
            case GrowingSpaceDemandClass.Intermediate: return IntermediateWeight;
            case GrowingSpaceDemandClass.High: return HighWeight;
            default:
                throw new ArgumentOutOfRangeException(nameof(demand), (int)demand, "unknown growing-space demand class");
        }
    }

    public static bool AnyNonReference(IReadOnlyList<DensityStem> stems)
    {
        for (int i = 0; i < stems.Count; i++)
            if (stems[i].Demand != GrowingSpaceDemandClass.Reference)
                return true;
        return false;
    }

    // The single place that decides the route. Models 0 and 1, and a model-2 stand with only Reference stems,
    // keep the original Sitka implementation.
    public static DensityRoute SelectRoute(bool speciesAwareGrowthModel, bool anyNonReferenceStem)
    {
        return speciesAwareGrowthModel && anyNonReferenceStem ? DensityRoute.SpeciesAwareOccupancy : DensityRoute.LegacySitkaExact;
    }

    // ----- occupancy: WHEN -----

    // One stem's occupancy in m2 of Reference-equivalent basal area: demand weight x pi/4 x DBH^2.
    public static double StemOccupancyM2(DensityStem stem)
    {
        double dbhM = stem.DbhCm / 100.0;
        return Weight(stem.Demand) * (Math.PI / 4.0) * dbhM * dbhM;
    }

    private static double OccupancyFraction(double occupancyM2, float areaHectares)
    {
        return areaHectares <= 0f ? 0.0 : occupancyM2 / areaHectares / OccupancyCapacityM2PerHa;
    }

    // Stand occupancy, 1.0 = capacity. Stems are added in ordinal tree-id order, so the result does not depend on
    // the order the caller lists them.
    public static double Occupancy(IReadOnlyList<DensityStem> stems, float areaHectares)
    {
        double occupancyM2 = 0.0;
        foreach (int i in CanonicalOrder(stems))
            occupancyM2 += StemOccupancyM2(stems[i]);
        return OccupancyFraction(occupancyM2, areaHectares);
    }

    // 0 at or below the onset, 1 at capacity, above 1 beyond capacity.
    public static double OvercrowdingPressure(double occupancy)
    {
        return Math.Max(0.0, (occupancy - OvercrowdingOnset) / (1.0 - OvercrowdingOnset));
    }

    // Expected fraction of the stand lost in a year at this pressure.
    public static double ExpectedAnnualLoss(double pressure)
    {
        return pressure <= 0.0 ? 0.0 : OvercrowdingStrength * pressure * pressure;
    }

    // ----- victims: WHICH -----

    // 0..1 from the species-neutral Hegyi index, with one common half saturation.
    public static double Suppression(double competitionIndex)
    {
        double s = 1.0 - 1.0 / (1.0 + Math.Max(0.0, competitionIndex) / NeutralHalfSaturationCi);
        return Math.Max(0.0, Math.Min(1.0, s));
    }

    public static double Vulnerability(double suppression)
    {
        return Math.Pow(Math.Max(0.0, Math.Min(1.0, suppression)), VulnerabilityExponent);
    }

    // Annual loss probability of one stem. Depends on pressure and competitive position only, never on class.
    public static double AnnualLossProbability(double expectedAnnualLoss, double suppression, double meanVulnerability)
    {
        if (expectedAnnualLoss <= 0.0 || meanVulnerability <= 0.0)
            return 0.0;
        return Math.Min(MaximumAnnualLossProbability, expectedAnnualLoss * Vulnerability(suppression) / meanVulnerability);
    }

    // ----- one annual step -----

    // roll(treeId) is the per-tree uniform [0,1) draw, so no random stream is consumed and the result does not
    // depend on call order. Pressure first (occupancy -> pressure -> expected loss), then victims.
    public static DensityMortalityPlan Plan(IReadOnlyList<DensityStem> stems, float areaHectares, Func<string, double> roll)
    {
        var plan = new DensityMortalityPlan();
        if (stems.Count == 0 || areaHectares <= 0f)
            return plan;

        int[] order = CanonicalOrder(stems);
        double occupancyM2 = 0.0, squaredDbhSum = 0.0;
        foreach (int i in order)
        {
            occupancyM2 += StemOccupancyM2(stems[i]);
            squaredDbhSum += (double)stems[i].DbhCm * stems[i].DbhCm;
        }
        plan.StemsPerHectare = stems.Count / areaHectares;
        plan.QuadraticMeanDbhCm = Math.Sqrt(squaredDbhSum / stems.Count);
        plan.Occupancy = OccupancyFraction(occupancyM2, areaHectares);
        plan.OccupancyAfter = plan.Occupancy;
        plan.OvercrowdingPressure = OvercrowdingPressure(plan.Occupancy);
        if (plan.OvercrowdingPressure <= 0.0)
            return plan;

        double expectedLoss = ExpectedAnnualLoss(plan.OvercrowdingPressure);
        var suppression = new double[stems.Count];
        double vulnerabilitySum = 0.0;
        foreach (int i in order)
        {
            suppression[i] = Suppression(stems[i].CompetitionIndex);
            vulnerabilitySum += Vulnerability(suppression[i]);
        }
        double meanVulnerability = vulnerabilitySum / stems.Count;

        var survivors = new List<int>(stems.Count);
        double survivingM2 = 0.0;
        foreach (int i in order)
        {
            double probability = AnnualLossProbability(expectedLoss, suppression[i], meanVulnerability);
            if (probability > 0.0 && roll(stems[i].TreeId) < probability)
            {
                plan.VictimIndices.Add(i);
            }
            else
            {
                survivors.Add(i);
                survivingM2 += StemOccupancyM2(stems[i]);
            }
        }
        plan.HazardDeaths = plan.VictimIndices.Count;

        // Boundary: occupancy may not stay above capacity. The most suppressed survivors die until it is back on it.
        if (OccupancyFraction(survivingM2, areaHectares) > 1.0)
        {
            var boundary = new List<int>(survivors);
            boundary.Sort((a, b) =>
            {
                int bySuppression = suppression[b].CompareTo(suppression[a]);
                if (bySuppression != 0) return bySuppression;
                int byId = string.CompareOrdinal(stems[a].TreeId, stems[b].TreeId);
                return byId != 0 ? byId : a.CompareTo(b);
            });
            int remaining = survivors.Count;
            foreach (int i in boundary)
            {
                if (remaining <= 1 || OccupancyFraction(survivingM2, areaHectares) <= 1.0)
                    break;
                survivingM2 -= StemOccupancyM2(stems[i]);
                remaining--;
                plan.VictimIndices.Add(i);
                plan.BoundaryDeaths++;
            }
        }
        plan.OccupancyAfter = OccupancyFraction(survivingM2, areaHectares);
        return plan;
    }

    // Indices of the stems in ascending ordinal tree-id order (index breaks exact ties).
    private static int[] CanonicalOrder(IReadOnlyList<DensityStem> stems)
    {
        var order = new int[stems.Count];
        for (int i = 0; i < order.Length; i++)
            order[i] = i;
        Array.Sort(order, (a, b) =>
        {
            int byId = string.CompareOrdinal(stems[a].TreeId, stems[b].TreeId);
            return byId != 0 ? byId : a.CompareTo(b);
        });
        return order;
    }
}
