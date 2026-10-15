using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

// Offline decomposition and scan of single-year generic-mortality pulses. No Unity. Runs the PRODUCTION planner
// (MixedDensityModel.PlanComponents) through the reduced stand simulator, on the same fixtures as the density checks.
//
//   one    <fixture> <L40|E80> <mode> [seed] [top N] [csv]     decompose the worst years of one run
//   scan   <L40|E80> <fixtures,..> <modes,..> <seeds N>        one line per run: worst one-year BA loss and the R1-R4 verdict
//
// For every year the decomposition reports what the generic component asked for and which stems paid for it:
//   requested fraction / expected count / whole-stem quota; stems, basal area and occupancy before and after;
//   victims by DBH class (count and basal area), victims of at least 20 cm;
//   recruits (stems that entered as new adults during the run and are still under 20 cm) against established stems;
//   suppression rank of each victim among all stems (1 = most suppressed) and the share of the allocation weight that sat
//   in stems under 20 cm.
//
// RECRUITMENT MODES (the reduced simulator has no regeneration model; recruits come from an external series):
//   smooth       an even flow (what every earlier offline check used; it cannot produce a recruit shortage)
//   pulse:P      the same mean inflow delivered as one cohort every P years
//   gap:A-B      no recruit in years A..B
//   none         no recruit ever
//
// This is a screening and explanation tool. It says which mechanism exists and how large it can get; it never says how
// the game behaves. Only the Unity harness counts for that.
public static class CanopyPulseDiagnostic
{
    static string F(double v, string fmt = "0.000") => v.ToString(fmt, CultureInfo.InvariantCulture);

    // CCF_CAL="Lmax,k,m[,basalAreaBudget]" replaces the production calibration for a sensitivity run; a budget of 1 switches the
    // basal-area budget off. Unset means the production default.
    static GenericResponseCalibration Calibration()
    {
        string env = Environment.GetEnvironmentVariable("CCF_CAL");
        if (string.IsNullOrEmpty(env)) return GenericResponseCalibration.Default;
        string[] p = env.Split(',');
        double D(int i) => double.Parse(p[i], CultureInfo.InvariantCulture);
        return p.Length >= 4 ? new GenericResponseCalibration(D(1), D(0), D(2), D(3)) : new GenericResponseCalibration(D(1), D(0), D(2));
    }

    sealed class Pulse
    {
        public int Year, N, PassedOver, Quota, Victims, SmallBefore, RecruitBefore, SmallVictims, BigVictims, BigStems;
        public double LargestStemShare, ExpectedCount, GenericFraction, OccBefore, OccAfter, BaBefore, BaDead, BigBa, BigBaTotal, Overshoot, BigVictimSizeRatio;
        public double SmallWeightShare, MedianVictimRank, WorstVictimRank, StemLoss, BaLoss;
        public int[] ClassVictims = new int[4];
        public double[] ClassBa = new double[4];
    }

    static readonly string[] ClassNames = { "<10", "10-20", "20-40", ">=40" };
    static int ClassOf(float dbh) => dbh < 10f ? 0 : dbh < 20f ? 1 : dbh < 40f ? 2 : 3;
    static double Ba(float dbhCm) => Math.PI / 4.0 * (dbhCm / 100.0) * (dbhCm / 100.0);

    static List<Pulse> Run(string fixture, string geo, string mode, int seed, GenericResponseCalibration cal, out List<SimYear> years)
    {
        SimConfig c = DensityCalibrationSearch.ConfigFor(fixture, geo, cal, seed, mode);
        var pulses = new List<Pulse>();
        c.OnVictims = (year, trees, ci, victims, rec) =>
        {
            DensityComponentPlan plan = rec.Plan;
            var pulse = new Pulse { Year = year, N = trees.Count, Victims = victims.Count };
            if (plan != null)
            {
                pulse.GenericFraction = plan.GenericLossFraction;
                pulse.ExpectedCount = plan.GenericLossFraction * plan.EligibleForGeneric;
                pulse.Quota = plan.GenericQuota;
                pulse.PassedOver = plan.GenericBudgetPassedOver;
                pulse.OccBefore = plan.Occupancy;
            }
            var weights = new double[trees.Count];
            double weightTotal = 0.0, weightSmall = 0.0, largest = 0.0;
            for (int i = 0; i < trees.Count; i++)
            {
                pulse.BaBefore += Ba(trees[i].Dbh);
                largest = Math.Max(largest, Ba(trees[i].Dbh));
                bool small = trees[i].Dbh < 20f;
                if (!small) { pulse.BigStems++; pulse.BigBaTotal += Ba(trees[i].Dbh); }
                if (small) pulse.SmallBefore++;
                if (small && trees[i].BornYear > 0) pulse.RecruitBefore++;
                weights[i] = MixedDensityModel.Vulnerability(MixedDensityModel.Suppression(ci[i]));
                weightTotal += weights[i];
                if (small) weightSmall += weights[i];
            }
            pulse.SmallWeightShare = weightTotal > 0 ? weightSmall / weightTotal : 0.0;
            pulse.LargestStemShare = pulse.BaBefore > 0 ? largest / pulse.BaBefore : 0.0;
            int[] rank = new int[trees.Count];
            int[] byWeight = Enumerable.Range(0, trees.Count).OrderByDescending(i => weights[i]).ThenBy(i => trees[i].Id, StringComparer.Ordinal).ToArray();
            for (int k = 0; k < byWeight.Length; k++) rank[byWeight[k]] = k + 1;
            var victimRanks = new List<int>();
            foreach (int i in victims)
            {
                double ba = Ba(trees[i].Dbh);
                pulse.BaDead += ba;
                int cls = ClassOf(trees[i].Dbh);
                pulse.ClassVictims[cls]++; pulse.ClassBa[cls] += ba;
                if (trees[i].Dbh < 20f) pulse.SmallVictims++;
                else { pulse.BigVictims++; pulse.BigBa += ba; }
                victimRanks.Add(rank[i]);
            }
            victimRanks.Sort();
            pulse.MedianVictimRank = victimRanks.Count > 0 ? victimRanks[victimRanks.Count / 2] : 0;
            pulse.WorstVictimRank = victimRanks.Count > 0 ? victimRanks[victimRanks.Count - 1] : 0;
            pulse.Overshoot = Math.Max(0, pulse.Quota - pulse.SmallBefore);
            // size of the canopy stems that died against the average canopy stem: 1.0 = picked like a random canopy stem
            pulse.BigVictimSizeRatio = pulse.BigVictims > 0 && pulse.BigStems > 0 ? (pulse.BigBa / pulse.BigVictims) / (pulse.BigBaTotal / pulse.BigStems) : 0.0;
            pulse.StemLoss = pulse.N > 0 ? (double)pulse.Victims / pulse.N : 0.0;
            pulse.BaLoss = pulse.BaBefore > 0 ? pulse.BaDead / pulse.BaBefore : 0.0;
            pulses.Add(pulse);
        };
        years = DensityTrajectorySim.Run(c);
        for (int i = 0; i < pulses.Count; i++) pulses[i].OccAfter = years[i].Occupancy;
        return pulses;
    }

    static void One(string[] a)
    {
        string fixture = a[1], geo = a[2], mode = a[3];
        int seed = a.Length > 4 ? int.Parse(a[4], CultureInfo.InvariantCulture) : 20261014;
        int top = a.Length > 5 ? int.Parse(a[5], CultureInfo.InvariantCulture) : 6;
        List<Pulse> pulses = Run(fixture, geo, mode, seed, Calibration(), out List<SimYear> years);
        var late = pulses.Where(p => p.Year >= 26).ToList();
        Pulse worst = late.OrderByDescending(p => p.BaLoss).First();
        Console.WriteLine($"== {fixture} {geo} recruitment={mode} seed={seed}");
        Console.WriteLine($"   years 26-300: max one-year BA loss {F(worst.BaLoss)} (year {worst.Year}); years with BA loss >= 0.15: {late.Count(p => p.BaLoss >= 0.15)}, >= 0.25: {late.Count(p => p.BaLoss >= 0.25)}; " +
                          $"max stem loss {F(late.Max(p => p.StemLoss))}; mean BA loss {F(late.Average(p => p.BaLoss))}; mean stem loss {F(late.Average(p => p.StemLoss))}");
        Console.WriteLine($"   years in which the whole-stem quota exceeded the stems under 20 cm: {late.Count(p => p.Overshoot > 0)} of {late.Count}; years with >=20 cm victims: {late.Count(p => p.BigVictims > 0)}");
        var withBig = late.Where(p => p.BigVictims > 0).ToList();
        if (withBig.Count > 0)
            Console.WriteLine($"   canopy (>=20 cm) victims: mean size against the average canopy stem {F(withBig.Average(p => p.BigVictimSizeRatio), "0.00")} (1.00 = chosen like a random canopy stem); share of allocation weight carried by stems under 20 cm in those years {F(withBig.Average(p => p.SmallWeightShare), "0.00")}");
        Console.WriteLine("   year    n  occB  occA  genFrac expCnt quota | small recr | victims small(>=20cm) | BAloss stemLoss | classes (n / BA m2) | rank med/worst | smallWtShare");
        foreach (Pulse p in late.OrderByDescending(p => p.BaLoss).Take(top))
        {
            string classes = string.Join(" ", Enumerable.Range(0, 4).Select(k => $"{ClassNames[k]}:{p.ClassVictims[k]}/{F(p.ClassBa[k], "0.00")}"));
            Console.WriteLine($"   {p.Year,4} {p.N,4} {F(p.OccBefore, "0.00")}  {F(p.OccAfter, "0.00")}  {F(p.GenericFraction)} {F(p.ExpectedCount, "0.0"),5} {p.Quota,5} | {p.SmallBefore,4} {p.RecruitBefore,4} | {p.Victims,3} {p.SmallVictims,3}({p.BigVictims,2}) | {F(p.BaLoss)} {F(p.StemLoss)} | {classes} | {p.MedianVictimRank,4:0}/{p.WorstVictimRank,4:0} | {F(p.SmallWeightShare)}");
        }
        if (a.Length > 6 && a[6] == "csv")
        {
            Console.WriteLine("year,n,occBefore,occAfter,genericFraction,expectedCount,quota,small,recruits,victims,smallVictims,bigVictims,baLoss,stemLoss,baAfter");
            foreach (Pulse p in pulses)
                Console.WriteLine(string.Join(",", p.Year, p.N, F(p.OccBefore), F(p.OccAfter), F(p.GenericFraction, "0.0000"), F(p.ExpectedCount, "0.00"), p.Quota, p.SmallBefore, p.RecruitBefore, p.Victims, p.SmallVictims, p.BigVictims, F(p.BaLoss, "0.0000"), F(p.StemLoss, "0.0000"), F(years[p.Year - 1].BaM2Ha, "0.00")));
        }
    }

    // Years (all years, including the dense-start years) in which the generic component broke its rule: a stand of nine stems or more
    // must lose no more basal area than the budget (at least one stem always fits then, so the never-stall rule cannot apply); a
    // smaller stand may lose one stem even if that exceeds the budget, so it is held to max(budget, share of the largest stem).
    // Pure broadleaf stands have no Sitka component, so there every death is a generic death; mixed stands are not counted here (the
    // Sitka component's own deaths are in the same BA figure), so the check is only made for pure stands.
    static int BoundViolations(List<Pulse> pulses, bool mixed, GenericResponseCalibration cal)
    {
        if (mixed) return -1;
        return pulses.Count(p => (p.N >= 9 && p.BaLoss > cal.BasalAreaBudgetShare + 1e-9)
            || p.BaLoss > Math.Max(cal.BasalAreaBudgetShare, p.LargestStemShare) + 1e-9);
    }

    // One line per (fixture, mode, seed). Mixed stands report the generic component only for R2 (addendum A4), as the checks do.
    static void Scan(string[] a)
    {
        string geo = a[1];
        string[] fixtures = a[2].Split(','), modes = a[3].Split(',');
        int seeds = int.Parse(a[4], CultureInfo.InvariantCulture);
        Console.WriteLine("fixture,geo,mode,seed,maxBaLoss,maxBaLossYear,yrsBa15,yrsBa25,meanBaLoss,maxStemLoss,meanStemLoss,blockChange,slope201,slope151,deadBa25,bigLoss25,minBaRatio,y300y100,R1,R2,R3,R4,nY100,nY300,baY300,occY300,nAtWorst,boundViolations,year1BaLoss,maxOccY26,guardYears");
        foreach (string fixture in fixtures)
            foreach (string mode in modes)
                for (int s = 0; s < seeds; s++)
                {
                    int seed = s == 0 ? 20261014 : 7000 + s;
                    List<Pulse> pulses = Run(fixture, geo, mode, seed, Calibration(), out List<SimYear> years);
                    var late = pulses.Where(p => p.Year >= 26).ToList();
                    Pulse worst = late.OrderByDescending(p => p.BaLoss).First();
                    bool mixed = fixture.StartsWith("C_") || fixture.StartsWith("G_");
                    TrajectoryMetrics.VerdictT2 v = TrajectoryMetrics.EvaluateT2(years, mixed);
                    Console.WriteLine(string.Join(",", fixture, geo, mode, seed, F(worst.BaLoss), worst.Year, late.Count(p => p.BaLoss >= 0.15), late.Count(p => p.BaLoss >= 0.25),
                        F(late.Average(p => p.BaLoss), "0.0000"), F(late.Max(p => p.StemLoss)), F(late.Average(p => p.StemLoss)), F(v.MaxBlockChange26), F(v.SlopeY201to300, "0.0000"), F(v.SlopeY151to250, "0.0000"),
                        F(v.WorstWindowDeadBa), F(v.WorstWindowBigLoss), F(v.MinBaRatio25y, "0.000"), F(v.BaY300OverY100, "0.000"), v.R1 ? 1 : 0, v.R2 ? 1 : 0, v.R3 ? 1 : 0, v.R4 ? 1 : 0,
                        years[99].LivingAfter, years[299].LivingAfter, F(years[299].BaM2Ha, "0.0"), F(years[299].Occupancy, "0.00"),
                        worst.N, BoundViolations(pulses, mixed, Calibration()), F(pulses[0].BaLoss), F(years.Skip(25).Max(y => y.Occupancy), "0.00"), late.Count(p => p.PassedOver > 0)));
                }
    }

    public static int Main(string[] args)
    {
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        DensityCalibrationSearch.LoadSeriesFile(Environment.GetEnvironmentVariable("CCF_RECRUITMENT_SERIES"));
        if (args.Length >= 4 && args[0] == "one") { One(args); return 0; }
        if (args.Length >= 5 && args[0] == "scan") { Scan(args); return 0; }
        Console.Error.WriteLine("usage: one <fixture> <L40|E80> <smooth|pulse:P|gap:A-B|none> [seed] [top N] [csv]\n       scan <L40|E80> <fixture,..> <mode,..> <seeds N>");
        return 2;
    }
}
