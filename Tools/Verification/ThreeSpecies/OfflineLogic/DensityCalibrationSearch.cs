using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

// Command-line driver for the reduced stand simulator (DensityTrajectorySim.cs). No Unity.
//
//   hindcast  <unity-v2-csv-dir>                  run the v2 kernel on the Unity fixtures, set beside the Unity v2 long runs
//   search                                         predeclared candidate grid x pure fixtures (v3 kernel), R1-R4 verdicts
//   compare   [Lmax k m]                           v2 and v3 kernels side by side on all fixtures for one setting
//   diagnose  [Lmax k m]                           late-mixed decomposition, v2 against v3, against the pure-Sitka comparator
//   dump      <fixture> <geo> <kernel> [Lmax k m]  per-year CSV of one run
//
// Recruitment series: recruitment-series-v2.csv next to this file (measured in the Unity v2 long runs).
public static class DensityCalibrationSearch
{
    sealed class Fixture
    {
        public string Name, Geometry;
        public float Side;
        public float PerHa, DbhMin, Spread;
        public Func<int, SimSpecies> SpeciesOf;
        public string SeriesKey;
        public bool Mixed => Name.StartsWith("C_") || Name.StartsWith("G_");
    }

    static SimSpecies Three(int k) => k % 3 == 0 ? SimSpecies.Sitka : k % 3 == 1 ? SimSpecies.Beech : SimSpecies.Oak;

    static List<Fixture> Fixtures(string geometry)
    {
        float side = geometry == "E80" ? 80f : 40f;
        Fixture F(string name, float perHa, float dbhMin, float spread, Func<int, SimSpecies> sp, string series) =>
            new Fixture { Name = name, Geometry = geometry, Side = side, PerHa = perHa, DbhMin = dbhMin, Spread = spread, SpeciesOf = sp, SeriesKey = series };
        return new List<Fixture>
        {
            F("A_OAK_DENSE", 1000f, 24f, 10f, _ => SimSpecies.Oak, "A_OAK_DENSE"),
            F("B_BEECH_DENSE", 1300f, 24f, 10f, _ => SimSpecies.Beech, "B_BEECH_DENSE"),
            F("C_MIXED_DENSE", 1300f, 24f, 10f, Three, "C_MIXED_DENSE"),
            F("D_SITKA_DENSE", 1200f, 24f, 10f, _ => SimSpecies.Sitka, "D_SITKA_DENSE_MODEL2"),
            F("E_OAK_YOUNG", 2500f, 8f, 6f, _ => SimSpecies.Oak, "A_OAK_DENSE"),
            F("F_BEECH_YOUNG", 2500f, 8f, 6f, _ => SimSpecies.Beech, "B_BEECH_DENSE"),
            F("G_MIXED_YOUNG", 2500f, 8f, 6f, Three, "G_MIXED_YOUNG")
        };
    }

    // geometry|scenario -> per-year rate by 25-year block
    static Dictionary<string, double[]> series = new Dictionary<string, double[]>();

    public static void LoadSeriesFile(string path) => LoadSeries(path);

    static void LoadSeries(string path)
    {
        foreach (string line in File.ReadAllLines(path))
        {
            if (line.StartsWith("#") || line.StartsWith("geometry") || line.Trim().Length == 0) continue;
            string[] p = line.Split(',');
            string key = p[0] + "|" + p[1];
            if (!series.ContainsKey(key)) series[key] = new double[12];
            series[key][(int.Parse(p[2], CultureInfo.InvariantCulture) - 1) / 25] = double.Parse(p[4], CultureInfo.InvariantCulture);
        }
    }

    static SimConfig Config(Fixture f, SimKernel kernel, GenericResponseCalibration generic, int years = 300, int seed = 20261014)
    {
        string key = f.Geometry + "|" + f.SeriesKey;
        double[] rates = series.TryGetValue(key, out double[] r) ? r : new double[12];
        double cumulative = 0.0; int issued = 0;
        var c = new SimConfig { SideMeters = f.Side, StemsPerHectare = f.PerHa, DbhMinCm = f.DbhMin, DbhSpreadCm = f.Spread, SpeciesOf = f.SpeciesOf,
            Years = years, Kernel = kernel, Generic = generic, Seed = seed };
        c.RecruitsFor = (year, trees) =>
        {
            cumulative += rates[Math.Min(11, (year - 1) / 25)];
            int total = (int)Math.Floor(cumulative + 1e-9);
            int n = total - issued;
            issued = total;
            var list = new List<SimSpecies>();
            for (int i = 0; i < n; i++)
            {
                if (!f.Mixed) { list.Add(f.SpeciesOf(0)); continue; }
                // seed sources: basal area of the stems of at least 20 cm of each species
                double s = 0, b = 0, o = 0;
                foreach (SimTree t in trees)
                {
                    if (t.Dbh < 20f) continue;
                    double ba = t.Dbh * t.Dbh;
                    if (t.Species.Prefix == "S") s += ba; else if (t.Species.Prefix == "B") b += ba; else o += ba;
                }
                double sum = s + b + o;
                if (sum <= 0) continue;
                double u = DensityTrajectorySim.Roll("RSP" + year + "-" + i + "-" + issued, year, 77) * sum;
                list.Add(u < s ? SimSpecies.Sitka : u < s + b ? SimSpecies.Beech : SimSpecies.Oak);
            }
            return list;
        };
        return c;
    }

    static string F(double v, string fmt = "0.00") => v.ToString(fmt, CultureInfo.InvariantCulture);

    static void Main2(string[] args)
    {
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
        string here = Path.GetDirectoryName(typeof(DensityCalibrationSearch).Assembly.Location);
        string seriesPath = Environment.GetEnvironmentVariable("CCF_RECRUITMENT_SERIES");
        LoadSeries(seriesPath);
        string cmd = args.Length > 0 ? args[0] : "search";
        GenericResponseCalibration Cal(int from) => args.Length >= from + 3
            ? new GenericResponseCalibration(double.Parse(args[from + 1]), double.Parse(args[from]), double.Parse(args[from + 2]))
            : GenericResponseCalibration.Default;
        switch (cmd)
        {
            case "hindcast": Hindcast(args[1]); break;
            case "search": Search(); break;
            case "search2": Search2(); break;
            case "compare": Compare(Cal(1)); break;
            case "diagnose": Diagnose(Cal(1)); break;
            case "dump": Dump(args[1], args[2], args[3] == "v2" ? SimKernel.V2Combined : SimKernel.V3Components, Cal(4)); break;
            case "exponent": Exponent(Cal(1)); break;
            default: Console.Error.WriteLine("unknown command " + cmd); Environment.Exit(2); break;
        }
    }

    public static int Main(string[] args) { Main2(args); return 0; }

    static void Dump(string fixture, string geo, SimKernel kernel, GenericResponseCalibration cal)
    {
        Fixture f = Fixtures(geo).First(x => x.Name.StartsWith(fixture));
        var y = DensityTrajectorySim.Run(Config(f, kernel, cal));
        Console.WriteLine("year,livingBefore,deaths,sitkaComponent,generic,recruits,livingAfter,occupancyUsed,occupancyAfter,baM2Ha,dq,deadBaFraction,bigBefore,bigDeaths,sitkaBefore,sitkaDeaths,sitkaRdBefore,nS,nB,nO");
        foreach (SimYear r in y)
            Console.WriteLine(string.Join(",", r.Year, r.LivingBefore, r.Deaths, r.SitkaComponentDeaths, r.GenericDeaths, r.Recruits, r.LivingAfter, F(r.OccupancyUsed, "0.000"), F(r.Occupancy, "0.000"), F(r.BaM2Ha, "0.0"), F(r.DqCm, "0.0"), F(r.DeadBaFraction, "0.0000"), r.BigBefore, r.BigDeaths, r.SitkaBefore, r.SitkaDeaths, F(r.SitkaRelativeDensityBefore, "0.000"), r.SitkaLiving, r.BeechLiving, r.OakLiving));
    }

    static string Row(List<SimYear> y, int year) => $"n{y[year - 1].LivingAfter,4} BA{y[year - 1].BaM2Ha,5:0.0} occ{y[year - 1].Occupancy,5:0.00} Dq{y[year - 1].DqCm,5:0.0}";

    static void Hindcast(string unityDir)
    {
        var cal = GenericResponseCalibration.Default;
        foreach (string geo in new[] { "E80", "L40" })
        {
            string file = Path.Combine(unityDir, $"longrun-{geo}-300y.csv");
            var unity = File.ReadAllLines(file).Skip(1).Select(l => l.Split(',')).GroupBy(p => p[0]).ToDictionary(g => g.Key, g => g.ToList());
            foreach (Fixture f in Fixtures(geo))
            {
                string un = f.SeriesKey;
                if (!unity.ContainsKey(un) || f.Name.StartsWith("E_") || f.Name.StartsWith("F_")) continue;
                var y = DensityTrajectorySim.Run(Config(f, SimKernel.V2Combined, cal));
                Console.WriteLine($"== {geo} {f.Name}  (v2 kernel, reduced simulator  vs  Unity v2 long run)");
                foreach (int year in new[] { 25, 50, 100, 200, 300 })
                {
                    string[] u = unity[un][year - 1];
                    Console.WriteLine($"  Y{year,3}: sim {Row(y, year)}   unity n{int.Parse(u[11]),4} BA{double.Parse(u[13]),5:0.0} occ{double.Parse(u[14]),5:0.00} Dq{double.Parse(u[15]),5:0.0}");
                }
                var v = TrajectoryMetrics.Evaluate(y);
                var uOcc = unity[un].Select(p => double.Parse(p[14])).ToList();
                Console.WriteLine($"  occupancy slope Y201-300: sim {F(v.SlopeY201to300, "0.0000")}  unity {F(TrajectoryMetrics.Slope(uOcc, 201, 300), "0.0000")}");
            }
        }
    }

    sealed class Candidate { public double Lmax, K, M; public string Label => $"Lmax{Lmax:0.00} k{K:0.00} m{M:0}"; }

    static List<Candidate> Grid()
    {
        var list = new List<Candidate>();
        foreach (double m in new[] { 2.0 })
            foreach (double lmax in new[] { 0.25, 0.35, 0.50 })
                foreach (double k in new[] { 0.20, 0.40, 0.80, 1.60 })
                    list.Add(new Candidate { Lmax = lmax, K = k, M = m });
        foreach (double lmax in new[] { 0.25, 0.35, 0.50 })
            foreach (double k in new[] { 0.10, 0.20, 0.40 })
                list.Add(new Candidate { Lmax = lmax, K = k, M = 1.0 });
        return list;
    }

    // Trial 2: amended metrics (CompositeDensityV3Predeclared.md). Pure fixtures decide; mixed fixtures are reported.
    static void Search2()
    {
        var pure = new[] { "A_OAK_DENSE", "B_BEECH_DENSE", "E_OAK_YOUNG", "F_BEECH_YOUNG" };
        var grid = Grid();
        var passed = new Dictionary<string, bool>();
        var margins = new Dictionary<string, double>();
        foreach (Candidate cand in grid)
        {
            var cal = new GenericResponseCalibration(cand.K, cand.Lmax, cand.M);
            bool all = true; double minMargin = 99; var cells = new List<string>(); var fails = new List<string>();
            foreach (string geo in new[] { "L40", "E80" })
                foreach (Fixture f in Fixtures(geo).Where(x => pure.Contains(x.Name)))
                {
                    var v = TrajectoryMetrics.EvaluateT2(DensityTrajectorySim.Run(Config(f, SimKernel.V3Components, cal)));
                    all &= v.All; minMargin = Math.Min(minMargin, v.MinMargin);
                    string tag = $"{(v.R1 ? "1" : "-")}{(v.R2 ? "2" : "-")}{(v.R3 ? "3" : "-")}{(v.R4 ? "4" : "-")}";
                    if (!v.All) fails.Add($"{geo}/{f.Name.Substring(0, 1)}:{tag}");
                    cells.Add($"{geo}/{f.Name.Substring(0, 1)} s{F(v.SlopeY201to300, "0.0000")}/{F(v.SlopeY151to250, "0.0000")} blk{F(v.MaxBlockChange26)} yr1 {F(v.FirstYearLoss * 100, "0")}% dBA{F(v.WorstWindowDeadBa * 100, "0.0")} big{F(v.WorstWindowBigLoss * 100, "0.0")} {tag}");
                }
            passed[cand.Label] = all; margins[cand.Label] = minMargin;
            Console.WriteLine($"{cand.Label,-18} {(all ? "PASS" : "fail")} minMargin {F(minMargin)} {(all ? "" : "failing: " + string.Join(" ", fails))}");
            if (Environment.GetEnvironmentVariable("CCF_VERBOSE") == "1") Console.WriteLine("    " + string.Join(" | ", cells));
        }
        // neighbours: one step in k and in Lmax, same exponent
        Console.WriteLine("-- passing set and neighbour support");
        foreach (Candidate cand in grid.Where(g => passed[g.Label]))
        {
            var same = grid.Where(g => g.M == cand.M).ToList();
            double[] ks = same.Select(g => g.K).Distinct().OrderBy(x => x).ToArray(), ls = same.Select(g => g.Lmax).Distinct().OrderBy(x => x).ToArray();
            int ki = Array.IndexOf(ks, cand.K), li = Array.IndexOf(ls, cand.Lmax);
            var neighbours = new List<Candidate>();
            if (ki > 0) neighbours.Add(same.First(g => g.K == ks[ki - 1] && g.Lmax == cand.Lmax));
            if (ki < ks.Length - 1) neighbours.Add(same.First(g => g.K == ks[ki + 1] && g.Lmax == cand.Lmax));
            if (li > 0) neighbours.Add(same.First(g => g.K == cand.K && g.Lmax == ls[li - 1]));
            if (li < ls.Length - 1) neighbours.Add(same.First(g => g.K == cand.K && g.Lmax == ls[li + 1]));
            bool supported = neighbours.All(n => passed[n.Label]);
            Console.WriteLine($"{cand.Label,-18} margin {F(margins[cand.Label])} neighbours {neighbours.Count(n => passed[n.Label])}/{neighbours.Count} {(supported ? "FULL SUPPORT" : "")}");
        }
    }

    // Acceptance of the PRODUCTION default calibration on the reduced simulator (Trial 2 definitions). Used by the offline suite.
    public static void Acceptance(Action<bool, string, string> check, Func<double, string, string> f)
    {
        var cal = GenericResponseCalibration.Default;
        var pure = new[] { "A_OAK_DENSE", "B_BEECH_DENSE", "E_OAK_YOUNG", "F_BEECH_YOUNG" };
        foreach (string geo in new[] { "L40", "E80" })
            foreach (Fixture fx in Fixtures(geo).Where(x => pure.Contains(x.Name)))
            {
                var y = DensityTrajectorySim.Run(Config(fx, SimKernel.V3Components, cal));
                var v = TrajectoryMetrics.EvaluateT2(y);
                Console.WriteLine($"LONG_RUN_TABLE {geo} {fx.Name} Y100 {Row(y, 100)} | Y200 {Row(y, 200)} | Y300 {Row(y, 300)} | firstYearLoss {f(v.FirstYearLoss * 100, "0")}% allStemLoss25y {f(v.WorstWindowStemLoss * 100, "0")}% recruits/yr {f(v.MeanRecruitsPerYear, "0.0")}");
                check(v.R1 && v.R2 && v.R3 && v.R4, $"LONG_RUN_{geo}_{fx.Name}_MEETS_R1_R4",
                    $"slope201-300={f(v.SlopeY201to300, "0.0000")} slope151-250={f(v.SlopeY151to250, "0.0000")} blockChange={f(v.MaxBlockChange26, "0.000")} deadBA25y={f(v.WorstWindowDeadBa * 100, "0.0")}% bigStemLoss25y={f(v.WorstWindowBigLoss * 100, "0.0")}% baY300/Y100={f(v.BaY300OverY100, "0.00")}");
            }
        foreach (string geo in new[] { "L40", "E80" })
            foreach (string name in new[] { "C_MIXED_DENSE", "G_MIXED_YOUNG" })
            {
                Fixture fx = Fixtures(geo).First(x => x.Name == name);
                var y = DensityTrajectorySim.Run(Config(fx, SimKernel.V3Components, cal));
                var v = TrajectoryMetrics.EvaluateT2(y, mixed: true);
                Console.WriteLine($"LONG_RUN_TABLE {geo} {name} Y100 {Row(y, 100)} | Y200 {Row(y, 200)} | Y300 {Row(y, 300)} | Sitka/Beech/Oak living Y300 {y[299].SitkaLiving}/{y[299].BeechLiving}/{y[299].OakLiving}");
                check(v.R2 && v.R3 && v.R4, $"LONG_RUN_{geo}_{name}_MEETS_R2_R4_WITH_R2_ON_THE_GENERIC_COMPONENT", $"genericBlockChange={f(v.MaxBlockChange26, "0.000")} deadBA25y={f(v.WorstWindowDeadBa * 100, "0.0")}% bigStemLoss25y={f(v.WorstWindowBigLoss * 100, "0.0")}% slope201-300={f(v.SlopeY201to300, "0.0000")}");
                check(v.MeanRdSitkaY201to300 <= 1.02, $"LONG_RUN_{geo}_{name}_SITKA_SUBSET_IS_HELD_AT_ITS_OWN_LINE_R5PRIME", $"meanRdBeforeStep={f(v.MeanRdSitkaY201to300, "0.000")} limit=1.02");
                // replay
                var y2 = DensityTrajectorySim.Run(Config(fx, SimKernel.V3Components, cal));
                bool same = y.Count == y2.Count && y.Zip(y2, (a, b) => a.LivingAfter == b.LivingAfter && a.Deaths == b.Deaths && a.Occupancy == b.Occupancy).All(x => x);
                check(same, $"LONG_RUN_{geo}_{name}_REPLAYS_EXACTLY", "");
            }
        foreach (string geo in new[] { "L40", "E80" })
        {
            // Pure Sitka is the same under both kernels, bit for bit.
            Fixture fx = Fixtures(geo).First(x => x.Name.StartsWith("D_"));
            var a = DensityTrajectorySim.Run(Config(fx, SimKernel.V2Combined, cal)); var b = DensityTrajectorySim.Run(Config(fx, SimKernel.V3Components, cal));
            check(a.Zip(b, (p, q) => p.LivingAfter == q.LivingAfter && p.Deaths == q.Deaths && p.Occupancy == q.Occupancy && p.BaM2Ha == q.BaM2Ha).All(x => x), $"LONG_RUN_{geo}_PURE_SITKA_IS_IDENTICAL_UNDER_THE_V2_AND_V3_KERNELS", "");
        }
        // Negative control: the v2 setting under the same acceptance must fail on pure Beech (it did in Unity), so the test can fail.
        var v2cal = new GenericResponseCalibration(0.20, 0.25, 2.0);
        var beech = Fixtures("E80").First(x => x.Name == "B_BEECH_DENSE");
        var vb = TrajectoryMetrics.EvaluateT2(DensityTrajectorySim.Run(Config(beech, SimKernel.V3Components, v2cal)));
        check(!vb.R1, "NEGATIVE_CONTROL_THE_V2_GENERIC_SETTING_FAILS_THE_PURE_BEECH_FLATTENING_TEST", $"slope201-300={f(vb.SlopeY201to300, "0.0000")} (Unity v2 measured +0.0054)");
    }

    static void Search()
    {
        var pure = new[] { "A_OAK_DENSE", "B_BEECH_DENSE", "E_OAK_YOUNG", "F_BEECH_YOUNG" };
        Console.WriteLine("candidate, then per fixture/geometry: slope201-300 | occ300/occ200 | worst25y deadBA% | worst25y stemLoss% | verdict (R1R2R3R4), margin");
        foreach (Candidate cand in Grid())
        {
            var cal = new GenericResponseCalibration(cand.K, cand.Lmax, cand.M);
            var cells = new List<string>();
            bool all = true; double minMargin = 99;
            foreach (string geo in new[] { "L40", "E80" })
                foreach (Fixture f in Fixtures(geo).Where(x => pure.Contains(x.Name)))
                {
                    var y = DensityTrajectorySim.Run(Config(f, SimKernel.V3Components, cal));
                    var v = TrajectoryMetrics.Evaluate(y);
                    all &= v.All; minMargin = Math.Min(minMargin, v.MinMargin);
                    cells.Add($"{geo}/{f.Name.Substring(0, 1)}: s{F(v.SlopeY201to300, "0.0000")} r{F(v.OccY300OverY200)} d{F(v.WorstWindowDeadBa * 100, "0.0")} l{F(v.WorstWindowStemLoss * 100, "0")} {(v.R1 ? "1" : "-")}{(v.R2 ? "2" : "-")}{(v.R3 ? "3" : "-")}{(v.R4 ? "4" : "-")}");
                }
            Console.WriteLine($"{cand.Label,-18} {(all ? "PASS" : "fail")} minMargin {F(minMargin)} | {string.Join(" | ", cells)}");
        }
    }

    // DIAGNOSTIC: how far the victim-allocation exponent decides whether the generic loss reaches canopy trees.
    static void Exponent(GenericResponseCalibration cal)
    {
        Console.WriteLine($"setting Lmax {cal.MaximumLoss} k {cal.Strength} m {cal.Exponent}; pure Beech dense and young; exponent = victim-allocation exponent (production 4)");
        foreach (string geo in new[] { "L40", "E80" })
            foreach (string name in new[] { "B_BEECH_DENSE", "F_BEECH_YOUNG" })
                foreach (double exponent in new[] { 4.0, 2.0, 1.0, 0.0 })
                {
                    Fixture f = Fixtures(geo).First(x => x.Name == name);
                    SimConfig cfg = Config(f, SimKernel.V3Components, cal);
                    cfg.DiagnosticVulnerabilityExponent = exponent;
                    var y = DensityTrajectorySim.Run(cfg);
                    var v = TrajectoryMetrics.Evaluate(y);
                    double Big(int a, int b) => y.Skip(a - 1).Take(b - a + 1).Sum(r => r.BigDeaths) / (double)Math.Max(1, y.Skip(a - 1).Take(b - a + 1).Sum(r => r.BigBefore));
                    Console.WriteLine($"{geo} {name,-13} exp {exponent,3:0.0}: Y100 {Row(y, 100)} | Y200 {Row(y, 200)} | Y300 {Row(y, 300)} | slope201-300 {F(v.SlopeY201to300, "0.0000")} | big-stem mortality Y101-200 {F(Big(101, 200) * 100)}% Y201-300 {F(Big(201, 300) * 100)}% | deadBA25y {F(v.WorstWindowDeadBa * 100, "0.0")}%");
                }
    }

    static void Compare(GenericResponseCalibration cal)
    {
        Console.WriteLine($"setting Lmax {cal.MaximumLoss} k {cal.Strength} m {cal.Exponent}");
        foreach (string geo in new[] { "L40", "E80" })
            foreach (Fixture f in Fixtures(geo))
                foreach (SimKernel kernel in new[] { SimKernel.V2Combined, SimKernel.V3Components })
                {
                    var y = DensityTrajectorySim.Run(Config(f, kernel, cal));
                    var v = TrajectoryMetrics.Evaluate(y);
                    Console.WriteLine($"{geo} {f.Name,-14} {(kernel == SimKernel.V2Combined ? "v2" : "v3")}  Y25 {Row(y, 25)} | Y100 {Row(y, 100)} | Y200 {Row(y, 200)} | Y300 {Row(y, 300)} | slope201-300 {F(v.SlopeY201to300, "0.0000")} deadBA25y {F(v.WorstWindowDeadBa * 100, "0.0")}% loss25y {F(v.WorstWindowStemLoss * 100, "0")}% {(v.All ? "R1-4 pass" : "R" + (v.R1 ? "" : "1") + (v.R2 ? "" : "2") + (v.R3 ? "" : "3") + (v.R4 ? "" : "4") + " fail")}");
                }
    }

    // Late-mixed decomposition (predeclared section 6): who pays the Sitka debt, what the Sitka subset does, how the loss compares with pure Sitka.
    static void Diagnose(GenericResponseCalibration cal)
    {
        foreach (string geo in new[] { "L40", "E80" })
        {
            var pureSitka = DensityTrajectorySim.Run(Config(Fixtures(geo).First(x => x.Name.StartsWith("D_")), SimKernel.V3Components, cal));
            double Window(List<SimYear> y, int a, int b, Func<SimYear, double> f) => y.Skip(a - 1).Take(b - a + 1).Average(f);
            double sitkaLossPure = Window(pureSitka, 201, 300, r => r.SitkaBefore > 0 ? (double)r.SitkaDeaths / r.SitkaBefore : 0.0);
            Console.WriteLine($"== {geo}: pure Sitka comparator Y201-300  Sitka-stem loss/yr {F(sitkaLossPure, "0.000")}  stems {F(Window(pureSitka, 201, 300, r => r.LivingBefore), "0")}  RD_S {F(Window(pureSitka, 201, 300, r => r.SitkaRelativeDensityBefore), "0.00")}");
            foreach (string name in new[] { "C_MIXED_DENSE", "G_MIXED_YOUNG" })
                foreach (SimKernel kernel in new[] { SimKernel.V2Combined, SimKernel.V3Components })
                {
                    var y = DensityTrajectorySim.Run(Config(Fixtures(geo).First(x => x.Name == name), kernel, cal));
                    foreach (var (a, b) in new[] { (101, 150), (151, 200), (201, 250), (251, 300) })
                    {
                        double living = Window(y, a, b, r => r.LivingBefore), deaths = Window(y, a, b, r => r.Deaths);
                        double sitkaLiving = Window(y, a, b, r => r.SitkaBefore), sitkaDeaths = Window(y, a, b, r => r.SitkaDeaths);
                        double demanded = Window(y, a, b, r => r.SitkaComponentDeaths);
                        double rd = Window(y, a, b, r => r.SitkaRelativeDensityBefore);
                        Console.WriteLine($"  {name} {(kernel == SimKernel.V2Combined ? "v2" : "v3")} Y{a}-{b}: living {F(living, "0"),4} deaths/yr {F(deaths, "0.0"),5} ({F(deaths / Math.Max(1, living), "0.000")}) | Sitka stems {F(sitkaLiving, "0"),4} Sitka deaths/yr {F(sitkaDeaths, "0.0"),5} ({F(sitkaDeaths / Math.Max(1, sitkaLiving), "0.000")} per Sitka stem) of {F(demanded, "0.0")} demanded of the Sitka subset | RD_S before {F(rd, "0.00")} | generic deaths/yr {F(Window(y, a, b, r => r.GenericDeaths), "0.0")}");
                    }
                }
        }
    }
}
