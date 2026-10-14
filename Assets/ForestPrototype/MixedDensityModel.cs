// V2 controller uses PlanComposite (MixedDensityModel.Composite.cs). The legacy generic Plan below
// remains an independently tested occupancy primitive; it is no longer the full mixed controller route.
using System;
using System.Collections.Generic;

// Adult density for growth model 2 (D-059, D-061). Two routes handle the annual density step:
//
//   LegacySitkaExact       Every living adult is Reference class (Sitka). ForestEcologyController keeps running the
//                          original growth-model-1 code: the British Sitka size-density line, its hazard and its
//                          boundary (SitkaGrowthModel). Nothing in this file runs, so pure Sitka is model 1 exactly.
//   SpeciesAwareOccupancy  At least one living adult is not Reference class (Oak, Beech). Everything below applies.
//
// This file uses NOTHING from SitkaGrowthModel (the offline check compiles it without that file), and
// SitkaGrowthModel uses nothing from this file (it compiles without it). Changing any number here cannot change
// pure Sitka.
//
// WHAT THIS IS. A dimensionless SIMULATION OCCUPANCY abstraction. It is a game mechanism for "how crowded is this
// mixed stand, and how much does that thin it". It is NOT an SDI, NOT Reineke's rule, NOT the British Sitka relative
// density, NOT a maximum basal area and NOT a maximum-density law for any species. No evidence for an Oak or Beech
// maximum density is available, none is invented here, and occupancy 1.0 is not a measured density of anything. It
// must be recalibrated if stronger broadleaf density evidence becomes available.
//
//   occupied space of a stem = demand weight x stem cross-section (pi/4 x DBH^2)
//   occupancy                = sum over living stems of occupied space / stand area / OccupancyNormalization
//   pressure                 = share of the occupancy that lies beyond the onset = max(0, 1 - 1/occupancy)
//   expected annual loss     = MaximumAnnualLoss x pressure          (a fraction of the stand's stems)
//
//   - Stem cross-section is plain geometry, a size footprint, not a fitted size-density exponent.
//   - The demand weight is the stem's ordinal growing-space-demand class (D-061). A higher class takes more of the
//     same space than a Reference stem of the same size. Species enter ONLY through this class, and ONLY in the
//     occupancy sum. The sum is plain addition: no DEC, no DMC, no overyielding, no mixture bonus.
//   - Pressure starts at occupancy 1.0 by definition: OccupancyNormalization is the number that decides where that
//     is. It is a divisor, not a limit. Nothing stops occupancy exceeding 1.0; the response is a bounded annual loss.
//   - Pressure is below 1 and loss is below MaximumAnnualLoss however crowded the stand is, so the annual effect is
//     bounded. Below the onset there is no density mortality and no random draw is consumed.
//   - No species or class appears anywhere in the pressure or loss formulas.
//
// WHICH stems die is separate from how many. Only competitive position decides, never species or class: the existing
// Hegyi index (species-neutral: neighbour diameters and distances only) becomes suppression through ONE common half
// saturation, and risk is spread over stems in proportion to suppression^exponent so dominant stems rarely die. A
// species' own Ci50 is deliberately NOT used: it already sets that species' growth response, so using it again here
// double-counts it and removes Oak/Beech first at equal competition. Differential mortality would need its own
// evidence.
//
// Calibration (labels: [ORD] ordinal rank, [C] simulation calibration, none of it a measurement):
//   - class weights 1.0 / 1.5 / 2.0: [ORD] rank, [C] magnitude, provisional (D-061). Not Oak or Beech data.
//   - OccupancyNormalization 55 and MaximumAnnualLoss 0.10: [C], the two numbers that set occupancy and loss. They
//     were chosen only so this route lines up with the accepted Sitka behaviour near representative Scenario One
//     Sitka structures (see Docs/Verification/ThreeSpeciesProgramme/Phase1-DensityRework.md), so adding the first Oak
//     to a Sitka stand does not make its Sitka part suddenly immune or suddenly doomed. That is a continuity choice,
//     not Sitka evidence for other species and not a claim about any species.
//   - VulnerabilityExponent 4 and NeutralHalfSaturationCi 5: [C], the victim-allocation seam. Initial simulation
//     calibration inherited numerically from growth model 1 for continuity (it keeps dominant stems from being
//     picked). They decide which stems die, never how many. They are not supported mortality constants.
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

// The two numbers that set where occupancy pressure starts and how large the annual loss can get. Passed explicitly
// so a check can change them and show what moves; production code always uses Default.
public readonly struct OccupancyCalibration
{
    // Divisor of the weighted occupied space per hectare. Decides which stands reach occupancy 1.0 (the onset).
    public readonly double Normalization;
    // Upper bound of the expected annual loss fraction, approached only as occupancy grows without limit.
    public readonly double MaximumAnnualLoss;

    public OccupancyCalibration(double normalization, double maximumAnnualLoss)
    {
        if (!(normalization > 0.0))
            throw new ArgumentOutOfRangeException(nameof(normalization), normalization, "normalization must be positive");
        if (!(maximumAnnualLoss > 0.0 && maximumAnnualLoss < 1.0))
            throw new ArgumentOutOfRangeException(nameof(maximumAnnualLoss), maximumAnnualLoss, "maximum annual loss must be in (0, 1)");
        Normalization = normalization;
        MaximumAnnualLoss = maximumAnnualLoss;
    }

    public static OccupancyCalibration Default => new OccupancyCalibration(MixedDensityModel.OccupancyNormalization, MixedDensityModel.MaximumAnnualLoss);
}

// Outcome of one annual step. Victims are indices into the stem list the plan was made from, in ordinal tree-id order.
public sealed class DensityMortalityPlan
{
    public double StemsPerHectare;          // all living stems (descriptive)
    public double QuadraticMeanDbhCm;       // all living stems pooled (descriptive)
    public double Occupancy;                // 1.0 = where overcrowding pressure starts
    public double OvercrowdingPressure;     // 0 .. below 1
    public double ExpectedAnnualLoss;       // fraction of the stand's stems, below MaximumAnnualLoss
    public int HazardDeaths;
    public readonly List<int> VictimIndices = new List<int>();
}

public static partial class MixedDensityModel
{
    // [ORD/C] class weights. Coarse, deliberately round ordinal steps, not fitted values.
    public const double ReferenceWeight = 1.0;
    public const double IntermediateWeight = 1.5;
    public const double HighWeight = 2.0;

    // [C] divisor that makes occupancy dimensionless. Not a limit, not a basal-area ceiling, no forestry meaning of
    // its own; it only places the onset. See the header for how it was chosen.
    public const double OccupancyNormalization = 55.0;
    // Occupancy at which pressure starts. A definition, not a calibration: OccupancyNormalization moves the stands.
    public const double OnsetOccupancy = 1.0;
    // [C] upper bound of the expected annual loss fraction.
    public const double MaximumAnnualLoss = 0.10;
    // [C] victim-allocation seam, inherited numerically from growth model 1 for continuity: risk goes as suppression^exponent.
    public const double VulnerabilityExponent = 4.0;
    // [C] victim-allocation seam: one Hegyi half-saturation for every species, so suppression does not depend on species.
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

    // ----- occupancy: how crowded -----

    // One stem's occupied space: demand weight x cross-section pi/4 x DBH^2, in m2 of Reference-equivalent cross-section.
    public static double StemOccupiedSpace(DensityStem stem)
    {
        double dbhM = stem.DbhCm / 100.0;
        return Weight(stem.Demand) * (Math.PI / 4.0) * dbhM * dbhM;
    }

    private static double OccupancyOf(double occupiedSpace, float areaHectares, OccupancyCalibration calibration)
    {
        return areaHectares <= 0f ? 0.0 : occupiedSpace / areaHectares / calibration.Normalization;
    }

    public static double Occupancy(IReadOnlyList<DensityStem> stems, float areaHectares)
    {
        return Occupancy(stems, areaHectares, OccupancyCalibration.Default);
    }

    // Stems are added in ordinal tree-id order, so the result does not depend on the order the caller lists them.
    public static double Occupancy(IReadOnlyList<DensityStem> stems, float areaHectares, OccupancyCalibration calibration)
    {
        double occupiedSpace = 0.0;
        foreach (int i in CanonicalOrder(stems))
            occupiedSpace += StemOccupiedSpace(stems[i]);
        return OccupancyOf(occupiedSpace, areaHectares, calibration);
    }

    // ----- pressure and loss: how much -----

    // The share of the occupancy that lies beyond the onset: 0 at or below it, rising towards (never reaching) 1.
    public static double OvercrowdingPressure(double occupancy)
    {
        return occupancy <= OnsetOccupancy ? 0.0 : 1.0 - OnsetOccupancy / occupancy;
    }

    // Expected fraction of the stand's stems lost in a year: zero at no pressure, increasing with it, always below
    // the maximum.
    public static double ExpectedAnnualLoss(double pressure, OccupancyCalibration calibration)
    {
        return pressure <= 0.0 ? 0.0 : calibration.MaximumAnnualLoss * Math.Min(1.0, pressure);
    }

    // ----- victims: which -----

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

    // Annual loss probability of one stem. Depends on the stand's expected loss and the stem's competitive position
    // only, never on class or species.
    public static double AnnualLossProbability(double expectedAnnualLoss, double suppression, double meanVulnerability)
    {
        if (expectedAnnualLoss <= 0.0 || meanVulnerability <= 0.0)
            return 0.0;
        return Math.Min(1.0, expectedAnnualLoss * Vulnerability(suppression) / meanVulnerability);
    }

    // ----- one annual step -----

    public static DensityMortalityPlan Plan(IReadOnlyList<DensityStem> stems, float areaHectares, Func<string, double> roll)
    {
        return Plan(stems, areaHectares, roll, OccupancyCalibration.Default);
    }

    // roll(treeId) is the per-tree uniform [0,1) draw, so no random stream is consumed and the result does not
    // depend on call order. Occupancy -> pressure -> expected loss first, then which stems.
    public static DensityMortalityPlan Plan(IReadOnlyList<DensityStem> stems, float areaHectares, Func<string, double> roll, OccupancyCalibration calibration)
    {
        var plan = new DensityMortalityPlan();
        if (stems.Count == 0 || areaHectares <= 0f)
            return plan;

        int[] order = CanonicalOrder(stems);
        double occupiedSpace = 0.0, squaredDbhSum = 0.0;
        foreach (int i in order)
        {
            occupiedSpace += StemOccupiedSpace(stems[i]);
            squaredDbhSum += (double)stems[i].DbhCm * stems[i].DbhCm;
        }
        plan.StemsPerHectare = stems.Count / areaHectares;
        plan.QuadraticMeanDbhCm = Math.Sqrt(squaredDbhSum / stems.Count);
        plan.Occupancy = OccupancyOf(occupiedSpace, areaHectares, calibration);
        plan.OvercrowdingPressure = OvercrowdingPressure(plan.Occupancy);
        plan.ExpectedAnnualLoss = ExpectedAnnualLoss(plan.OvercrowdingPressure, calibration);
        if (plan.ExpectedAnnualLoss <= 0.0)
            return plan;

        var suppression = new double[stems.Count];
        double vulnerabilitySum = 0.0;
        foreach (int i in order)
        {
            suppression[i] = Suppression(stems[i].CompetitionIndex);
            vulnerabilitySum += Vulnerability(suppression[i]);
        }
        double meanVulnerability = vulnerabilitySum / stems.Count;

        foreach (int i in order)
        {
            double probability = AnnualLossProbability(plan.ExpectedAnnualLoss, suppression[i], meanVulnerability);
            if (probability > 0.0 && roll(stems[i].TreeId) < probability)
                plan.VictimIndices.Add(i);
        }
        plan.HazardDeaths = plan.VictimIndices.Count;
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
