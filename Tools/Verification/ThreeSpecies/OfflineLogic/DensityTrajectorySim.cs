using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

// Reduced, deterministic stand simulator for the mixed-density offline checks and the generic-response calibration search.
// No UnityEngine. It runs the PRODUCTION density code (SitkaSubsetDensitySignal, MixedDensityModel) on a stand whose
// trees grow with the production Hegyi competition and DBH equations:
//
//   competition index  CI_i = sum over neighbours within 8 m of (D_j / max(1, D_i)) / max(0.5, distance)   [float]
//   DBH growth         D += potential x site x clamp01(1 - D / Dmax) x 1 / (1 + CI / Ci50)                  [float]
//   density mortality  applied AFTER growth, with the competition index from BEFORE growth (as the annual step does)
//
// Species parameters are the accepted integrated species rows (Oak/Beech calibration 796da99).
// Historical density-v3 screening evidence used .4/.4 with Ci50=3: reproduce it at 90e3496, not this integrated input set.
// AssetGuards still requires exact agreement with the actual assets; no acceptance threshold changes. Fixtures reproduce the Unity qualification harness
// (regular grid, the same DBH pattern, the same ids), so a run here can be set beside the matching Unity run.
//
// WHAT IT DOES NOT HAVE: canopy light, seed rain, juvenile cohorts, browsing, site variation (site = 1) or crown
// geometry. New adult trees arrive from an EXTERNAL recruitment series (promotion size 3.5 m = 5.25 cm DBH, random
// position), measured on the Unity v2 long runs. It is a screening tool: it can say that a candidate cannot flatten a
// trajectory or that a mechanism exists, never that the game behaves a given way. Only the Unity harness counts for that.
public enum SimKernel
{
    // a7a759d: one combined quota, one allocator over every stem (the Sitka debt can fall on broadleaves).
    V2Combined = 0,
    // component-faithful: Sitka debt paid by Sitka stems, generic deaths drawn from the rest.
    V3Components = 1
}

public sealed class SimSpecies
{
    public string Prefix;
    public GrowingSpaceDemandClass Demand;
    public float PotentialDbhGrowth, MaxDbh, Ci50;

    public static readonly SimSpecies Sitka = new SimSpecies { Prefix = "S", Demand = GrowingSpaceDemandClass.Reference, PotentialDbhGrowth = 1.2f, MaxDbh = 120f, Ci50 = 5f };
    public static readonly SimSpecies Beech = new SimSpecies { Prefix = "B", Demand = GrowingSpaceDemandClass.Intermediate, PotentialDbhGrowth = 0.9f, MaxDbh = 120f, Ci50 = 6.5f };
    public static readonly SimSpecies Oak = new SimSpecies { Prefix = "O", Demand = GrowingSpaceDemandClass.High, PotentialDbhGrowth = 0.8f, MaxDbh = 120f, Ci50 = 5f };
}

public sealed class SimTree
{
    public string Id;
    public SimSpecies Species;
    public float X, Z, Dbh;
    // Year the stem entered the stand as a new adult (0 = part of the starting stand). Diagnostic only.
    public int BornYear;
}

public sealed class SimYear
{
    public int Year;
    public int LivingBefore, Deaths, SitkaComponentDeaths, GenericDeaths;
    public int Recruits;
    public int LivingAfter;
    public double Occupancy;          // after the step
    public double OccupancyUsed;      // the value the step saw
    public double BaM2Ha, DqCm;
    public double DeadBaFraction;     // dead basal area / living basal area before
    public int BigBefore, BigDeaths;  // stems of at least 20 cm DBH
    public int SitkaBefore, SitkaDeaths;
    public double SitkaRelativeDensityBefore;
    public int SitkaLiving, BeechLiving, OakLiving;
    // The generic-component plan of this year's step (null for pure Sitka or the v2 kernel). Diagnostic only.
    public DensityComponentPlan Plan;
}

public sealed class SimConfig
{
    public float SideMeters = 80f;
    public float StemsPerHectare = 1300f, DbhMinCm = 24f, DbhSpreadCm = 10f;
    // species of the k-th grid stem
    public Func<int, SimSpecies> SpeciesOf = _ => SimSpecies.Beech;
    public int Years = 300;
    public SimKernel Kernel = SimKernel.V3Components;
    public GenericResponseCalibration Generic = GenericResponseCalibration.Default;
    // new adult trees to add after the step of year y (1-based), given the stand after the step; null = none
    public Func<int, IReadOnlyList<SimTree>, List<SimSpecies>> RecruitsFor;
    public int Seed = 20261014;
    // DIAGNOSTIC ONLY (not a production lever): replaces the common victim-allocation exponent (4) in the generic draw.
    public double? DiagnosticVulnerabilityExponent;
    // DIAGNOSTIC ONLY: called after the victims are chosen and before they are removed, with the stand as the step saw it
    // (trees, pre-growth competition indices, victim indices). Must not change the stand.
    public Action<int, List<SimTree>, float[], List<int>, SimYear> OnVictims;

    public float AreaHectares => SideMeters * SideMeters / 10000f;
}

public static class DensityTrajectorySim
{
    public const float HegyiCutoffMeters = 8f;
    public const float RecruitDbhCm = 5.25f;    // JuvenileEcologyRules.PromotionDbhCm(3.5 m)

    // Deterministic keyed uniform [0,1): 64-bit FNV-1a of the key, mixed with year and seed. Same key, same number.
    public static double Roll(string key, int year, int seed)
    {
        unchecked
        {
            ulong h = 14695981039346656037UL;
            foreach (char c in key) { h ^= c; h *= 1099511628211UL; }
            h ^= (ulong)(uint)year * 0x9E3779B97F4A7C15UL;
            h ^= (ulong)(uint)seed * 0xC2B2AE3D27D4EB4FUL;
            h ^= h >> 30; h *= 0xBF58476D1CE4E5B9UL;
            h ^= h >> 27; h *= 0x94D049BB133111EBUL;
            h ^= h >> 31;
            return (h >> 11) / 9007199254740992.0;
        }
    }

    public static List<SimTree> BuildStand(SimConfig c)
    {
        int count = Math.Max(1, (int)Math.Round(c.StemsPerHectare * c.AreaHectares, MidpointRounding.AwayFromZero));
        int perAxis = (int)Math.Ceiling(Math.Sqrt(count));
        float spacing = (c.SideMeters - 2f) / perAxis;
        var trees = new List<SimTree>(count);
        for (int k = 0; k < count; k++)
        {
            SimSpecies species = c.SpeciesOf(k);
            trees.Add(new SimTree
            {
                Id = species.Prefix + k.ToString("0000", CultureInfo.InvariantCulture),
                Species = species,
                X = -c.SideMeters * 0.5f + 1f + (k / perAxis + 0.5f) * spacing,
                Z = -c.SideMeters * 0.5f + 1f + (k % perAxis + 0.5f) * spacing,
                Dbh = c.DbhMinCm + ((k * 7919) % 100) / 100f * c.DbhSpreadCm
            });
        }
        return trees;
    }

    public static List<SimYear> Run(SimConfig c)
    {
        List<SimTree> trees = BuildStand(c);
        var years = new List<SimYear>(c.Years);
        float area = c.AreaHectares;
        int recruitSerial = 0;
        for (int year = 1; year <= c.Years; year++)
        {
            var rec = new SimYear { Year = year, LivingBefore = trees.Count };
            // 1. competition from the current stand
            float[] ci = CompetitionIndices(trees);
            // 2. growth (uses the competition above)
            var grown = new float[trees.Count];
            for (int i = 0; i < trees.Count; i++)
            {
                SimTree t = trees[i];
                float potential = t.Species.PotentialDbhGrowth * 1f * Clamp01(1f - t.Dbh / t.Species.MaxDbh);
                grown[i] = potential * (1f / (1f + ci[i] / t.Species.Ci50));
            }
            for (int i = 0; i < trees.Count; i++) trees[i].Dbh += grown[i];

            // 3. density mortality on the grown diameters with the pre-growth competition
            var stems = new List<DensityStem>(trees.Count);
            for (int i = 0; i < trees.Count; i++)
                stems.Add(new DensityStem(trees[i].Id, trees[i].Species.Demand, trees[i].Dbh, ci[i]));
            double baBefore = BasalArea(trees) / area;
            rec.BaM2Ha = baBefore;
            var victims = ChooseVictims(c, stems, year, rec, area);
            c.OnVictims?.Invoke(year, trees, ci, victims, rec);
            double deadBa = 0.0;
            rec.BigBefore = trees.Count(t => t.Dbh >= 20f);
            rec.SitkaBefore = trees.Count(t => t.Species.Demand == GrowingSpaceDemandClass.Reference);
            foreach (int i in victims)
            {
                deadBa += Math.PI / 4.0 * (trees[i].Dbh / 100.0) * (trees[i].Dbh / 100.0);
                if (trees[i].Dbh >= 20f) rec.BigDeaths++;
                if (trees[i].Species.Demand == GrowingSpaceDemandClass.Reference) rec.SitkaDeaths++;
            }
            rec.DeadBaFraction = baBefore > 0 ? deadBa / area / baBefore : 0.0;
            rec.Deaths = victims.Count;
            var dead = new HashSet<int>(victims);
            var survivors = new List<SimTree>(trees.Count - victims.Count);
            for (int i = 0; i < trees.Count; i++) if (!dead.Contains(i)) survivors.Add(trees[i]);
            trees = survivors;

            // 4. new adult trees (external series)
            List<SimSpecies> recruits = c.RecruitsFor?.Invoke(year, trees);
            if (recruits != null)
                foreach (SimSpecies species in recruits)
                {
                    string id = "R" + year.ToString(CultureInfo.InvariantCulture) + "-" + (recruitSerial++).ToString(CultureInfo.InvariantCulture);
                    trees.Add(new SimTree
                    {
                        Id = id, Species = species, Dbh = RecruitDbhCm, BornYear = year,
                        X = (float)((Roll("RX" + id, year, c.Seed) - 0.5) * (c.SideMeters - 1f)),
                        Z = (float)((Roll("RZ" + id, year, c.Seed) - 0.5) * (c.SideMeters - 1f))
                    });
                }
            rec.Recruits = recruits?.Count ?? 0;
            rec.LivingAfter = trees.Count;
            var after = trees.Select(t => new DensityStem(t.Id, t.Species.Demand, t.Dbh, 0.0)).ToList();
            rec.Occupancy = MixedDensityModel.Occupancy(after, area);
            rec.BaM2Ha = BasalArea(trees) / area;
            rec.DqCm = trees.Count > 0 ? Math.Sqrt(trees.Sum(t => (double)t.Dbh * t.Dbh) / trees.Count) : 0.0;
            rec.SitkaLiving = trees.Count(t => t.Species.Prefix == "S");
            rec.BeechLiving = trees.Count(t => t.Species.Prefix == "B");
            rec.OakLiving = trees.Count(t => t.Species.Prefix == "O");
            years.Add(rec);
        }
        return years;
    }

    private static float Clamp01(float v) => v < 0f ? 0f : (v > 1f ? 1f : v);

    private static double BasalArea(List<SimTree> trees)
    {
        double sum = 0.0;
        foreach (SimTree t in trees) sum += Math.PI / 4.0 * (t.Dbh / 100.0) * (t.Dbh / 100.0);
        return sum;
    }

    // Production Hegyi index, float arithmetic, 8 m cutoff.
    public static float[] CompetitionIndices(List<SimTree> trees)
    {
        int n = trees.Count;
        var ci = new float[n];
        float cutoffSquared = HegyiCutoffMeters * HegyiCutoffMeters;
        for (int i = 0; i < n; i++)
        {
            float sum = 0f;
            float di = Math.Max(1f, trees[i].Dbh);
            for (int j = 0; j < n; j++)
            {
                if (j == i) continue;
                float dx = trees[j].X - trees[i].X, dz = trees[j].Z - trees[i].Z;
                float squared = dx * dx + dz * dz;
                if (squared > cutoffSquared * 1.00002f) continue;
                float distance = (float)Math.Sqrt(squared);
                if (distance > HegyiCutoffMeters) continue;
                sum += (trees[j].Dbh / di) / Math.Max(0.5f, distance);
            }
            ci[i] = sum;
        }
        return ci;
    }

    private static List<int> ChooseVictims(SimConfig c, List<DensityStem> stems, int year, SimYear rec, float area)
    {
        var victims = new List<int>();
        if (stems.Count == 0) return victims;
        Func<string, double> sitkaRoll = id => Roll("ADULT-MORT-" + id, year, c.Seed);
        Func<string, double> genericRoll = id => Roll("ADULT-MORT-GENERIC-" + id, year, c.Seed);
        SitkaComponentResult sitka = SitkaSubsetDensitySignal.Resolve(stems, area, sitkaRoll);
        rec.SitkaRelativeDensityBefore = sitka.RelativeDensity;
        if (!MixedDensityModel.AnyNonReference(stems))
        {
            // pure Sitka stand: the original route (the component equals the original block on an all-Sitka list)
            victims.AddRange(sitka.HazardVictimIndices);
            victims.AddRange(sitka.BoundaryVictimIndices);
            rec.SitkaComponentDeaths = victims.Count;
            rec.OccupancyUsed = 0.0;
            return victims;
        }
        rec.OccupancyUsed = MixedDensityModel.Occupancy(stems, area);
        if (c.Kernel == SimKernel.V3Components)
        {
            DensityComponentPlan plan = c.DiagnosticVulnerabilityExponent.HasValue
                ? DiagnosticKernel.Plan(stems, area, sitka.HazardVictimIndices, sitka.BoundaryVictimIndices, genericRoll, c.Generic, c.DiagnosticVulnerabilityExponent.Value)
                : MixedDensityModel.PlanComponents(stems, area, sitka.HazardVictimIndices, sitka.BoundaryVictimIndices, genericRoll, c.Generic);
            rec.SitkaComponentDeaths = plan.SitkaHazardDeaths + plan.SitkaBoundaryDeaths;
            rec.GenericDeaths = plan.GenericDeaths;
            rec.Plan = plan;
            victims.AddRange(plan.VictimIndices);
            return victims;
        }
        // v2 reference: the Sitka subset only supplies a COUNT; one allocator over every stem pays it.
        V2ReferenceKernel.Result v2 = V2ReferenceKernel.Plan(stems, area, sitka.Deaths, sitkaRoll, c.Generic);
        rec.SitkaComponentDeaths = sitka.Deaths;   // what was demanded of the Sitka subset
        rec.GenericDeaths = 0;                     // the single allocator does not separate them
        victims.AddRange(v2.VictimIndices);
        return victims;
    }
}

// The a7a759d (v2) planner, kept in the tools tree only so the offline checks and the calibration search can set v3 beside it.
// Literal copy of the arithmetic of the deleted MixedDensityModel.PlanComposite, with the generic response parameters passed in.
public static class V2ReferenceKernel
{
    public sealed class Result
    {
        public double ExpectedFraction, Occupancy, NonReferenceShare;
        public readonly List<int> VictimIndices = new List<int>();
    }

    public static Result Plan(IReadOnlyList<DensityStem> stems, float areaHectares, int referenceDeaths, Func<string, double> roll, GenericResponseCalibration generic)
    {
        var plan = new Result();
        if (stems.Count == 0 || areaHectares <= 0f) return plan;
        int[] order = Enumerable.Range(0, stems.Count).ToArray();
        Array.Sort(order, (a, b) => { int c = string.CompareOrdinal(stems[a].TreeId, stems[b].TreeId); return c != 0 ? c : a.CompareTo(b); });
        double occupied = 0.0, nonReference = 0.0;
        foreach (int i in order)
        {
            double space = MixedDensityModel.StemOccupiedSpace(stems[i]);
            occupied += space;
            if (stems[i].Demand != GrowingSpaceDemandClass.Reference) nonReference += space;
        }
        plan.Occupancy = MixedDensityModel.Occupancy(stems, areaHectares);
        plan.NonReferenceShare = occupied > 0.0 ? nonReference / occupied : 0.0;
        double g = MixedDensityModel.GenericLoss(plan.Occupancy, generic) * plan.NonReferenceShare;
        double reference = (double)referenceDeaths / stems.Count;
        plan.ExpectedFraction = 1.0 - (1.0 - reference) * (1.0 - g);
        double count = plan.ExpectedFraction * stems.Count;
        int quota = (int)Math.Floor(count + 1e-10);
        double remainder = Math.Max(0.0, count - quota);
        if (remainder > 0.0 && roll("COMPOSITE-QUOTA") < remainder) quota++;
        quota = Math.Min(stems.Count, Math.Max(referenceDeaths, quota));
        if (quota == 0) return plan;
        double total = 0.0;
        foreach (int i in order) total += MixedDensityModel.Vulnerability(MixedDensityModel.Suppression(stems[i].CompetitionIndex));
        var priority = new Dictionary<int, double>();
        foreach (int i in order)
        {
            double weight = total > 0.0 ? MixedDensityModel.Vulnerability(MixedDensityModel.Suppression(stems[i].CompetitionIndex)) : 1.0;
            double u = Math.Max(1e-15, Math.Min(1.0 - 1e-15, roll(stems[i].TreeId)));
            priority[i] = weight > 0.0 ? -Math.Log(u) / weight : double.PositiveInfinity;
        }
        Array.Sort(order, (a, b) => { int c = priority[a].CompareTo(priority[b]); return c != 0 ? c : string.CompareOrdinal(stems[a].TreeId, stems[b].TreeId); });
        for (int i = 0; i < quota; i++) plan.VictimIndices.Add(order[i]);
        plan.VictimIndices.Sort();
        return plan;
    }
}

// Metrics over a finished trajectory, shared by the offline checks and the calibration search. Predeclared in
// Docs/Verification/ThreeSpeciesProgramme/CompositeDensityV3Predeclared.md section 5.
public static class TrajectoryMetrics
{
    public static double Slope(IList<double> y, int firstYear, int lastYear)
    {
        int n = lastYear - firstYear + 1;
        double sx = 0, sy = 0, sxx = 0, sxy = 0;
        for (int year = firstYear; year <= lastYear; year++)
        {
            double x = year, v = y[year - 1];
            sx += x; sy += v; sxx += x * x; sxy += x * v;
        }
        double den = n * sxx - sx * sx;
        return Math.Abs(den) < 1e-12 ? 0.0 : (n * sxy - sx * sy) / den;
    }

    public sealed class Verdict
    {
        public double SlopeY201to300, SlopeY151to250, OccY300OverY200;
        public double MaxBlockChange, MaxYearLoss;
        public double WorstWindowDeadBa, WorstWindowStemLoss;
        public double MinBaRatio25y, BaY300OverY100;
        public bool R1, R2, R3, R4;
        // normalised margins: 1 = on the limit, > 1 = inside it by that factor; the minimum is the candidate's margin
        public double MinMargin;
        public bool All => R1 && R2 && R3 && R4;
    }

    // Trial 2 (CompositeDensityV3Predeclared.md, "Trial 2 definitions"): R1' slopes only, R2' from year 26, R3' dead basal area
    // and stems of at least 20 cm (all-stem loss and the recruit inflow are reported, not tested), R4 unchanged.
    public sealed class VerdictT2
    {
        public double SlopeY201to300, SlopeY151to250, OccY300OverY200;
        public double MaxBlockChange26, MaxYearLoss26, FirstYearLoss;
        public double WorstWindowDeadBa, WorstWindowBigLoss, WorstWindowStemLoss, MeanRecruitsPerYear;
        public double MinBaRatio25y, BaY300OverY100;
        public double MeanRdSitkaY201to300;
        public bool R1, R2, R3, R4;
        public double MinMargin;
        public bool All => R1 && R2 && R3 && R4;
    }

    public static VerdictT2 EvaluateT2(List<SimYear> y, bool mixed = false)
    {
        var v = new VerdictT2();
        var occ = y.Select(r => r.Occupancy).ToList();
        v.SlopeY201to300 = Slope(occ, 201, 300);
        v.SlopeY151to250 = Slope(occ, 151, 250);
        v.OccY300OverY200 = occ[299] / Math.Max(1e-9, occ[199]);
        var loss = y.Select(r => r.LivingBefore > 0 ? (double)r.Deaths / r.LivingBefore : 0.0).ToList();
        v.FirstYearLoss = loss[0];
        // Addendum A4: R2 of a MIXED stand is judged on the generic component only; the Sitka component is the unchanged legacy process.
        if (mixed) loss = y.Select(r => r.LivingBefore > 0 ? (double)r.GenericDeaths / r.LivingBefore : 0.0).ToList();
        v.MaxYearLoss26 = loss.Skip(25).Max();
        double maxChange = 0.0;
        for (int b = 26; b + 9 <= 300; b += 5)
            maxChange = Math.Max(maxChange, Math.Abs(loss.Skip(b + 4).Take(5).Average() - loss.Skip(b - 1).Take(5).Average()));
        v.MaxBlockChange26 = maxChange;
        double worstBa = 0.0, worstBig = 0.0, worstLoss = 0.0;
        for (int start = 26; start + 24 <= 300; start++)
        {
            var w = y.Skip(start - 1).Take(25).ToList();
            worstBa = Math.Max(worstBa, w.Average(r => r.DeadBaFraction));
            double big = (double)w.Sum(r => r.BigDeaths) / Math.Max(1, w.Sum(r => r.BigBefore));
            worstBig = Math.Max(worstBig, big);
            worstLoss = Math.Max(worstLoss, w.Sum(r => r.Deaths) / (double)Math.Max(1, w.Sum(r => r.LivingBefore)));
        }
        v.WorstWindowDeadBa = worstBa; v.WorstWindowBigLoss = worstBig; v.WorstWindowStemLoss = worstLoss;
        v.MeanRecruitsPerYear = y.Skip(200).Average(r => (double)r.Recruits);
        v.MeanRdSitkaY201to300 = y.Skip(200).Average(r => r.SitkaRelativeDensityBefore);
        double minRatio = 1.0;
        for (int i = 25; i < 300; i++) minRatio = Math.Min(minRatio, y[i].BaM2Ha / Math.Max(1e-9, y[i - 25].BaM2Ha));
        v.MinBaRatio25y = minRatio;
        v.BaY300OverY100 = y[299].BaM2Ha / Math.Max(1e-9, y[99].BaM2Ha);
        v.R1 = Math.Abs(v.SlopeY201to300) <= 0.0025 && Math.Abs(v.SlopeY151to250) <= 0.004;
        v.R2 = v.MaxBlockChange26 <= 0.10 && v.MaxYearLoss26 <= 0.40;
        v.R3 = v.WorstWindowDeadBa <= 0.04 && v.WorstWindowBigLoss <= 0.05;
        v.R4 = v.MinBaRatio25y >= 0.5 && v.BaY300OverY100 >= 0.5;
        double margin(double limit, double value) => value <= 1e-12 ? 10.0 : limit / value;
        v.MinMargin = new[]
        {
            margin(0.0025, Math.Abs(v.SlopeY201to300)), margin(0.004, Math.Abs(v.SlopeY151to250)),
            margin(0.10, v.MaxBlockChange26), margin(0.40, v.MaxYearLoss26),
            margin(0.04, v.WorstWindowDeadBa), margin(0.05, v.WorstWindowBigLoss),
            v.MinBaRatio25y / 0.5, v.BaY300OverY100 / 0.5
        }.Min();
        return v;
    }

    public static Verdict Evaluate(List<SimYear> y)
    {
        var v = new Verdict();
        var occ = y.Select(r => r.Occupancy).ToList();
        v.SlopeY201to300 = Slope(occ, 201, 300);
        v.SlopeY151to250 = Slope(occ, 151, 250);
        v.OccY300OverY200 = occ[299] / Math.Max(1e-9, occ[199]);
        var lossFraction = y.Select(r => r.LivingBefore > 0 ? (double)r.Deaths / r.LivingBefore : 0.0).ToList();
        v.MaxYearLoss = lossFraction.Max();
        double maxChange = 0.0;
        for (int b = 1; b + 9 < 300; b += 5)
        {
            double a = lossFraction.Skip(b - 1).Take(5).Average(), c2 = lossFraction.Skip(b + 4).Take(5).Average();
            maxChange = Math.Max(maxChange, Math.Abs(c2 - a));
        }
        v.MaxBlockChange = maxChange;
        double worstBa = 0.0, worstLoss = 0.0;
        for (int start = 26; start + 24 <= 300; start++)
        {
            worstBa = Math.Max(worstBa, y.Skip(start - 1).Take(25).Average(r => r.DeadBaFraction));
            worstLoss = Math.Max(worstLoss, lossFraction.Skip(start - 1).Take(25).Average());
        }
        v.WorstWindowDeadBa = worstBa; v.WorstWindowStemLoss = worstLoss;
        double minRatio = 1.0;
        for (int i = 25; i < 300; i++) minRatio = Math.Min(minRatio, y[i].BaM2Ha / Math.Max(1e-9, y[i - 25].BaM2Ha));
        v.MinBaRatio25y = minRatio;
        v.BaY300OverY100 = y[299].BaM2Ha / Math.Max(1e-9, y[99].BaM2Ha);
        v.R1 = Math.Abs(v.SlopeY201to300) <= 0.0025 && Math.Abs(v.SlopeY151to250) <= 0.004 && v.OccY300OverY200 <= 1.10;
        v.R2 = v.MaxBlockChange <= 0.10 && v.MaxYearLoss <= 0.40;
        v.R3 = v.WorstWindowDeadBa <= 0.04 && v.WorstWindowStemLoss <= 0.30;
        v.R4 = v.MinBaRatio25y >= 0.5 && v.BaY300OverY100 >= 0.5;
        double margin(double limit, double value) => value <= 1e-12 ? 10.0 : limit / value;
        v.MinMargin = new[]
        {
            margin(0.0025, Math.Abs(v.SlopeY201to300)), margin(0.004, Math.Abs(v.SlopeY151to250)), margin(1.10 - 1.0, Math.Max(0.0, v.OccY300OverY200 - 1.0)),
            margin(0.10, v.MaxBlockChange), margin(0.40, v.MaxYearLoss), margin(0.04, v.WorstWindowDeadBa), margin(0.30, v.WorstWindowStemLoss),
            v.MinBaRatio25y / 0.5, v.BaY300OverY100 / 0.5
        }.Min();
        return v;
    }
}


// DIAGNOSTIC ONLY. A copy of the generic draw in MixedDensityModel.PlanComponents with the victim-allocation exponent as an
// argument, to show how far the allocation seam (not the response) decides whether a loss reaches canopy trees. Never used
// by production code or by any acceptance check.
public static class DiagnosticKernel
{
    public static DensityComponentPlan Plan(IReadOnlyList<DensityStem> stems, float area, IReadOnlyList<int> hazard, IReadOnlyList<int> boundary,
        Func<string, double> genericRoll, GenericResponseCalibration calibration, double exponent)
    {
        var plan = new DensityComponentPlan();
        var taken = new bool[stems.Count];
        foreach (int i in hazard) taken[i] = true;
        foreach (int i in boundary) taken[i] = true;
        double occupied = 0, nonRef = 0;
        foreach (DensityStem s in stems) { double sp = MixedDensityModel.StemOccupiedSpace(s); occupied += sp; if (s.Demand != GrowingSpaceDemandClass.Reference) nonRef += sp; }
        double occupancy = MixedDensityModel.Occupancy(stems, area);
        double g = MixedDensityModel.GenericLoss(occupancy, calibration) * (occupied > 0 ? nonRef / occupied : 0.0);
        int eligible = stems.Count - hazard.Count - boundary.Count;
        double expected = g * eligible;
        int quota = (int)Math.Floor(expected + 1e-10);
        if (expected - quota > 0 && genericRoll(MixedDensityModel.GenericQuotaRollKey) < expected - quota) quota++;
        quota = Math.Min(eligible, quota);
        var list = new List<int>();
        for (int i = 0; i < stems.Count; i++) if (!taken[i]) list.Add(i);
        double Weight(int i) => Math.Pow(Math.Max(0.0, Math.Min(1.0, MixedDensityModel.Suppression(stems[i].CompetitionIndex))), exponent);
        double total = list.Sum(Weight);
        var pr = new Dictionary<int, double>();
        foreach (int i in list)
        {
            double w = total > 0 ? Weight(i) : 1.0;
            double u = Math.Max(1e-15, Math.Min(1 - 1e-15, genericRoll(stems[i].TreeId)));
            pr[i] = w > 0 ? -Math.Log(u) / w : double.PositiveInfinity;
        }
        list.Sort((a, b) => { int c = pr[a].CompareTo(pr[b]); return c != 0 ? c : string.CompareOrdinal(stems[a].TreeId, stems[b].TreeId); });
        plan.VictimIndices.AddRange(hazard); plan.VictimIndices.AddRange(boundary);
        plan.SitkaHazardDeaths = hazard.Count; plan.SitkaBoundaryDeaths = boundary.Count;
        for (int k = 0; k < quota; k++) { plan.VictimIndices.Add(list[k]); plan.GenericVictimIndices.Add(list[k]); }
        plan.GenericDeaths = plan.GenericVictimIndices.Count;
        return plan;
    }
}
