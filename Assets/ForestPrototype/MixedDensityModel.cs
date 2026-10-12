using System;
using System.Collections.Generic;

// Growth model 2 adult density for stands that are not pure Sitka (D-059, D-061).
//
// The calibrated relationship in SitkaGrowthModel is a British Sitka maximum
// size-density line [B]. It is NOT a measured Oak or Beech relationship and this
// file does not pretend to supply one. It states an ordinal assumption instead:
//
//   One stem of each species occupies a different amount of growing space than
//   a Sitka stem of the same size. Species are placed in three ordinal classes
//   (GrowingSpaceDemandClass) and each class carries a plain weight.
//
// Stand relative density is the SUM over species of that species' own relative
// density (its stems/ha and its own quadratic mean diameter through the Sitka
// relationship) multiplied by its class weight. Equivalent reading: a species of
// weight w reaches the same maximum line with 1/w times as many stems.
//
// Status of every number below (labels: [B] British transfer evidence, [C]
// calibration, [ORD] ordinal rank, not a measurement):
//   - the size-density slope, the maximum SDI and the mortality shape stay the
//     Sitka values in SitkaGrowthModel, applied to the weighted density;
//   - class weights 1.0 / 1.5 / 2.0 are [ORD] with [C] magnitude. They encode
//     "Oak and Beech reach density-dependent mortality at lower stem numbers than
//     Sitka". The ordering is qualitative; the magnitudes are not measured and
//     are not Irish or British Oak/Beech maximum-density data;
//   - there is no DEC, no DMC, no overyielding and no mixture bonus: species do
//     not interact beyond sharing the stand's weighted density.
//
// A stand whose living trees are all Reference class never reaches this file:
// the controller keeps the existing Sitka route for it, bit for bit.
//
// No UnityEngine types: the model runs and is verified outside the Editor.

// Ordinal growing-space demand of one stem relative to the calibrated Sitka
// reference. Serialized on TreeSpeciesDefinition (stored as an int in assets).
public enum GrowingSpaceDemandClass
{
    // The calibrated reference: the British Sitka size-density line applies unchanged.
    Reference = 0,
    // More growing space per stem than the reference.
    Intermediate = 1,
    // Most growing space per stem of the three classes.
    High = 2
}

// One living adult stem as the density model sees it.
public readonly struct DensityStem
{
    public readonly string TreeId;
    public readonly string SpeciesId;
    public readonly GrowingSpaceDemandClass Demand;
    public readonly float DbhCm;
    // Growth suppression 0..1 from the existing Hegyi response (species Ci50).
    public readonly double Suppression;

    public DensityStem(string treeId, string speciesId, GrowingSpaceDemandClass demand, float dbhCm, double suppression)
    {
        TreeId = treeId ?? "";
        SpeciesId = speciesId ?? "";
        Demand = demand;
        DbhCm = dbhCm;
        Suppression = suppression;
    }
}

// Outcome of one annual density step. Victims are indices into the stem list the
// plan was made from, in the order they die.
public sealed class DensityMortalityPlan
{
    public double StemsPerHectare;            // all living stems, unweighted
    public double QuadraticMeanDbhCm;         // all living stems pooled (diagnostic)
    public double UnweightedRelativeDensity;  // pooled Sitka relationship, as the Sitka-only route would see the stand
    public double RelativeDensity;            // demand-weighted; this drives the model
    public double DensityPressure;
    public int HazardDeaths;
    public int BoundaryDeaths;
    public readonly List<int> VictimIndices = new List<int>();
}

public static class MixedDensityModel
{
    // [ORD/C] class weights. Reference is exactly 1.0 so a Reference-only tally is
    // arithmetically the Sitka relationship. Intermediate and High are coarse,
    // deliberately round ordinal steps, not fitted values.
    public const double ReferenceWeight = 1.0;
    public const double IntermediateWeight = 1.5;
    public const double HighWeight = 2.0;

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

    // Running per-species sums. Species are kept in ordinal id order, so the
    // result does not depend on the order stems were added; Remove supports the
    // maximum-line boundary loop without rebuilding.
    public sealed class Tally
    {
        private sealed class SpeciesSum
        {
            public string SpeciesId;
            public double Weight;
            public int Count;
            public double SquaredDbhSum;
        }

        private readonly List<SpeciesSum> species = new List<SpeciesSum>(4);
        private int count;

        public int Count => count;

        public void Add(DensityStem stem)
        {
            SpeciesSum sum = Find(stem.SpeciesId, true);
            double weight = Weight(stem.Demand);
            if (sum.Count == 0)
                sum.Weight = weight;
            else if (sum.Weight != weight)
                throw new InvalidOperationException($"species {stem.SpeciesId} has stems in two growing-space demand classes");
            sum.Count++;
            sum.SquaredDbhSum += (double)stem.DbhCm * stem.DbhCm;
            count++;
        }

        public void Remove(DensityStem stem)
        {
            SpeciesSum sum = Find(stem.SpeciesId, false);
            if (sum == null || sum.Count <= 0)
                throw new InvalidOperationException($"no {stem.SpeciesId} stem to remove");
            sum.Count--;
            sum.SquaredDbhSum -= (double)stem.DbhCm * stem.DbhCm;
            count--;
        }

        // Sum over species of weight x (the Sitka relationship applied to that
        // species' own stems/ha and quadratic mean diameter). stems/ha divides an
        // int by the float stand area, exactly as the Sitka-only route does.
        public double RelativeDensity(float areaHectares)
        {
            if (areaHectares <= 0f)
                return 0.0;
            double total = 0.0;
            foreach (SpeciesSum sum in species)
            {
                if (sum.Count <= 0)
                    continue;
                double stemsPerHectare = sum.Count / areaHectares;
                double quadraticMeanDbh = Math.Sqrt(sum.SquaredDbhSum / sum.Count);
                total += sum.Weight * SitkaGrowthModel.RelativeDensity(stemsPerHectare, quadraticMeanDbh);
            }
            return total;
        }

        private SpeciesSum Find(string speciesId, bool create)
        {
            int low = 0, high = species.Count - 1;
            while (low <= high)
            {
                int mid = (low + high) >> 1;
                int order = string.CompareOrdinal(species[mid].SpeciesId, speciesId);
                if (order == 0)
                    return species[mid];
                if (order < 0)
                    low = mid + 1;
                else
                    high = mid - 1;
            }
            if (!create)
                return null;
            var added = new SpeciesSum { SpeciesId = speciesId };
            species.Insert(low, added);
            return added;
        }
    }

    public static double RelativeDensity(IReadOnlyList<DensityStem> stems, float areaHectares)
    {
        var tally = new Tally();
        for (int i = 0; i < stems.Count; i++)
            tally.Add(stems[i]);
        return tally.RelativeDensity(areaHectares);
    }

    // One annual density step for a stand with at least one non-Reference stem.
    // Same shape as the Sitka route: density pressure from the (here weighted)
    // relative density; each stem's annual death probability scaled by its own
    // suppression against the stand mean; then the maximum-line boundary removes
    // the most suppressed survivors (ties by tree id) until the weighted density
    // is back on the line. roll(treeId) is the per-tree uniform [0,1) draw, so no
    // random stream is consumed and the result does not depend on call order.
    public static DensityMortalityPlan Plan(IReadOnlyList<DensityStem> stems, float areaHectares, Func<string, double> roll)
    {
        var plan = new DensityMortalityPlan();
        if (stems.Count == 0 || areaHectares <= 0f)
            return plan;

        var all = new Tally();
        double squaredDbhSum = 0.0;
        for (int i = 0; i < stems.Count; i++)
        {
            all.Add(stems[i]);
            squaredDbhSum += (double)stems[i].DbhCm * stems[i].DbhCm;
        }
        plan.StemsPerHectare = stems.Count / areaHectares;
        plan.QuadraticMeanDbhCm = Math.Sqrt(squaredDbhSum / stems.Count);
        plan.UnweightedRelativeDensity = SitkaGrowthModel.RelativeDensity(plan.StemsPerHectare, plan.QuadraticMeanDbhCm);
        plan.RelativeDensity = all.RelativeDensity(areaHectares);
        plan.DensityPressure = SitkaGrowthModel.DensityPressure(plan.RelativeDensity);
        if (plan.DensityPressure <= 0.0)
            return plan;

        double vulnerabilitySum = 0.0;
        for (int i = 0; i < stems.Count; i++)
            vulnerabilitySum += SitkaGrowthModel.Vulnerability(stems[i].Suppression);
        double meanVulnerability = vulnerabilitySum / stems.Count;

        var survivors = new List<int>(stems.Count);
        for (int i = 0; i < stems.Count; i++)
        {
            double probability = SitkaGrowthModel.AnnualDeathProbability(plan.DensityPressure, stems[i].Suppression, meanVulnerability);
            if (probability > 0.0 && roll(stems[i].TreeId) < probability)
                plan.VictimIndices.Add(i);
            else
                survivors.Add(i);
        }
        plan.HazardDeaths = plan.VictimIndices.Count;

        var remaining = new Tally();
        foreach (int i in survivors)
            remaining.Add(stems[i]);
        if (remaining.RelativeDensity(areaHectares) > SitkaGrowthModel.MaximumRelativeDensity)
        {
            var order = new List<int>(survivors);
            order.Sort((a, b) =>
            {
                int bySuppression = stems[b].Suppression.CompareTo(stems[a].Suppression);
                return bySuppression != 0 ? bySuppression : string.CompareOrdinal(stems[a].TreeId, stems[b].TreeId);
            });
            foreach (int i in order)
            {
                if (remaining.Count <= 1 || remaining.RelativeDensity(areaHectares) <= SitkaGrowthModel.MaximumRelativeDensity)
                    break;
                remaining.Remove(stems[i]);
                plan.VictimIndices.Add(i);
                plan.BoundaryDeaths++;
            }
        }
        return plan;
    }
}
