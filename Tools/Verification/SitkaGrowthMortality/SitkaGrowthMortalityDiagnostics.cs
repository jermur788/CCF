using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif
using Debug = UnityEngine.Debug;

// Disposable diagnostics for task/sitka-site-height-adult-mortality.
// Copy into Assets/ForestPrototype, run with
// -executeMethod SitkaGrowthMortalityDiagnostics.Begin, then remove the copy and
// its .meta. Production code is NOT modified: mortality alternatives are
// prototypes applied by this harness through the existing ForestTree.ApplyMortality
// API, in memory only. Height candidates are evaluated offline from the exported
// per-tree snapshots (height does not feed competition, crowns or adult light).
//
// Stand: actual Scenario One starting-stand generator (authored age 20), RNG
// model 1, regeneration model 1, scenario browse pressure. One annual step is
// production ForestEcologyController.AdvanceOneYear.
public static class SitkaGrowthMortalityDiagnostics
{
#if UNITY_EDITOR
    public static void Begin()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        EditorApplication.isPlaying = true;
    }
#endif
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (Environment.GetCommandLineArgs().Any(a => a == "SitkaGrowthMortalityDiagnostics.Begin"))
            new GameObject("Sitka growth/mortality diagnostics").AddComponent<SitkaGrowthMortalityRunner>();
    }
}

public sealed class SitkaGrowthMortalityRunner : MonoBehaviour
{
    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
    private static readonly int[] Ages = { 20, 30, 40, 50, 60, 80, 100, 120 };
    private const int StartAge = 20;
    private const float PlotHectares = 0.16f; // 40 m x 40 m stand

    private ForestEcologyController e;
    private ForestStartingStand stand;
    private ForestTreeSpawner spawner;
    private ScenarioOneManager manager;
    private TreeSpeciesDefinition sitka;
    private string dir;
    private int years;
    private StreamWriter facts;

    private static string F(float v, string f = "0.###") => v.ToString(f, Inv);
    private static string F(double v, string f = "0.###") => v.ToString(f, Inv);
    private void Fact(string line) { facts.WriteLine(line); facts.Flush(); Debug.Log("SITKA_DIAG " + line); }

    private ForestTree[] Living() => FindObjectsByType<ForestTree>(FindObjectsSortMode.None)
        .Where(t => t.IsLiving).OrderBy(t => t.TreeId, StringComparer.Ordinal).ToArray();

    // ---------- regimes ----------

    private sealed class Regime
    {
        public string Name;
        public int[] Ages;          // stand ages at which a thinning is applied
        public float Fraction;      // share of living stems removed each time
        public bool FromBelow;      // true: smallest DBH first; false: most crowded (highest CI) first
    }

    private static readonly Regime[] Regimes =
    {
        new Regime { Name = "unthinned", Ages = new int[0], Fraction = 0f, FromBelow = true },
        new Regime { Name = "light_below", Ages = new[] { 25, 40 }, Fraction = 0.20f, FromBelow = true },
        new Regime { Name = "moderate_below", Ages = new[] { 25, 40, 55, 70 }, Fraction = 0.30f, FromBelow = true },
        new Regime { Name = "heavy_selective", Ages = new[] { 25, 40, 55, 70 }, Fraction = 0.35f, FromBelow = false },
    };

    private void Thin(Regime r)
    {
        ForestTree[] living = Living().Where(t => t.CanChop).ToArray();
        IEnumerable<ForestTree> order = r.FromBelow
            ? living.OrderBy(t => t.Diameter).ThenBy(t => t.TreeId, StringComparer.Ordinal)
            : living.OrderByDescending(t => e.GetCompetitionIndex(t)).ThenBy(t => t.TreeId, StringComparer.Ordinal);
        int count = Mathf.RoundToInt(living.Length * r.Fraction);
        e.BeginChangeBatch();
        foreach (ForestTree t in order.Take(count)) t.Fell();
        e.EndChangeBatch();
    }

    // ---------- mortality prototypes (not production) ----------
    // S = 1 - 1/(1 + CI/Ci50): the production competition response, i.e. the
    // fraction of potential DBH growth withheld this year.
    private const float SuppressionOnset = 0.5f;   // [I] no extra risk below 50% withheld growth
    private const float GrowthFloorCm = 0.15f;     // [I] chronic low realised DBH growth
    private const float ReinekeSlope = 1.605f;     // [B] Reineke (1933) self-thinning slope
    private const float SdiReferenceDqCm = 25f;

    private sealed class Mortality
    {
        public string Name;
        public float HazardMax;     // A/B/D annual hazard at maximum stress [I]
        public float SdiMax;        // C/D maximum stand density index per ha [I]; 0 = none
        public bool Suppression, LowGrowth;
    }

    private static readonly Mortality[] Mortalities =
    {
        new Mortality { Name = "E_none" },
        new Mortality { Name = "A_suppression", HazardMax = 0.10f, Suppression = true },
        new Mortality { Name = "B_low_growth", HazardMax = 0.10f, LowGrowth = true },
        new Mortality { Name = "C_self_thinning", SdiMax = 1800f },
        new Mortality { Name = "D_suppression_plus_cap", HazardMax = 0.10f, Suppression = true, SdiMax = 2200f },
    };

    private float Suppression(ForestTree t) => 1f - 1f / (1f + e.GetCompetitionIndex(t) / sitka.Ci50);

    private static float Sdi(ForestTree[] trees)
    {
        if (trees.Length == 0) return 0f;
        double dq = Math.Sqrt(trees.Average(t => (double)t.Diameter * t.Diameter));
        return (float)(trees.Length / PlotHectares * Math.Pow(dq / SdiReferenceDqCm, ReinekeSlope));
    }

    private int deadThisRun;
    private float deadwoodM3ThisRun;
    private readonly HashSet<string> deadIds = new HashSet<string>();

    private void Kill(ForestTree t, string cause)
    {
        float volume = t.BiologicalStemVolumeM3;
        if (!t.ApplyMortality(cause, e.EcologicalYear)) return;
        if (!deadIds.Add(t.TreeId)) throw new InvalidOperationException("duplicate death " + t.TreeId);
        deadThisRun++;
        deadwoodM3ThisRun += volume;
    }

    // Applied after each production annual step, on that step's competition.
    private void ApplyMortality(Mortality m)
    {
        if (m.Suppression || m.LowGrowth)
        {
            foreach (ForestTree t in Living())
            {
                float stress;
                if (m.Suppression)
                    stress = Mathf.Clamp01((Suppression(t) - SuppressionOnset) / (1f - SuppressionOnset));
                else
                    stress = Mathf.Clamp01((GrowthFloorCm - e.GetAnnualDbhGrowth(t)) / GrowthFloorCm);
                float hazard = m.HazardMax * stress * stress;
                if (hazard > 0f && SimulationRandom.Roll(SimulationRandom.MixedModel, "MORT-" + t.TreeId, e.EcologicalYear, e.SimulationSeed) < hazard)
                    Kill(t, m.Suppression ? "suppression" : "chronic low growth");
            }
        }
        if (m.SdiMax > 0f)
        {
            ForestTree[] living = Living();
            if (Sdi(living) > m.SdiMax)
            {
                // Most suppressed first (ties by ID): self-thinning removes the losers of competition.
                var queue = new Queue<ForestTree>(living.OrderByDescending(Suppression).ThenBy(t => t.TreeId, StringComparer.Ordinal));
                var remaining = new List<ForestTree>(living);
                while (queue.Count > 0 && Sdi(remaining.ToArray()) > m.SdiMax)
                {
                    ForestTree t = queue.Dequeue();
                    remaining.Remove(t);
                    Kill(t, "self-thinning");
                }
            }
        }
    }

    // ---------- stand set-up and metrics ----------

    private void Build()
    {
        foreach (ForestTree t in FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            DestroyImmediate(t.gameObject);
        e.ResetForDeterministicRun();
        e.RngModelVersion = SimulationRandom.MixedModel;
        e.RegenerationModelVersion = RegenerationModel.AgeBands;
        e.Browsing.BackgroundPressure = manager.Definition.BackgroundBrowsePressure;
        e.Browsing.ClearProtection();
        stand.Generate();
        e.ResetForDeterministicRun();
        e.RngModelVersion = SimulationRandom.MixedModel;
        e.RegenerationModelVersion = RegenerationModel.AgeBands;
        e.InvalidateCompetition();
        deadThisRun = 0; deadwoodM3ThisRun = 0f; deadIds.Clear();
    }

    private string StandLine(string label, int age, ForestTree[] trees)
    {
        int n = trees.Length;
        if (n == 0) return string.Join(",", label, age, 0);
        var byDbh = trees.OrderByDescending(t => t.Diameter).ThenBy(t => t.TreeId, StringComparer.Ordinal).ToArray();
        var byH = trees.OrderByDescending(t => t.Height).ThenBy(t => t.TreeId, StringComparer.Ordinal).ToArray();
        int top = Mathf.Max(1, Mathf.RoundToInt(100 * PlotHectares)); // 100 largest-DBH stems per hectare
        float topHeight = byDbh.Take(top).Average(t => t.Height);
        float dominantHeight = byH.Take(top).Average(t => t.Height);
        float[] h = trees.Select(t => t.Height).OrderBy(x => x).ToArray();
        float[] d = trees.Select(t => t.Diameter).OrderBy(x => x).ToArray();
        float[] hd = trees.Select(t => t.Height * 100f / Mathf.Max(0.1f, t.Diameter)).OrderBy(x => x).ToArray();
        float[] ci = trees.Select(t => e.GetCompetitionIndex(t)).OrderBy(x => x).ToArray();
        float Q(float[] a, float q) => a[Mathf.Clamp(Mathf.FloorToInt(q * (a.Length - 1)), 0, a.Length - 1)];
        double ba = trees.Sum(t => Math.PI / 4.0 * Math.Pow(t.Diameter / 100.0, 2)) / PlotHectares;
        double dq = Math.Sqrt(trees.Average(t => (double)t.Diameter * t.Diameter));
        double vol = trees.Sum(t => (double)t.BiologicalStemVolumeM3) / PlotHectares;
        int recruits = trees.Count(t => !t.TreeId.StartsWith("P", StringComparison.Ordinal));
        float topHd = byDbh.Take(top).Average(t => t.Height * 100f / t.Diameter);
        return string.Join(",", label, age, n, F(n / PlotHectares, "0"), recruits, F(topHeight, "0.00"), F(dominantHeight, "0.00"),
            F(h.Average(), "0.00"), F(Q(h, 0.1f), "0.00"), F(Q(h, 0.5f), "0.00"), F(Q(h, 0.9f), "0.00"),
            F(d.Average(), "0.00"), F(dq, "0.00"), F(Q(d, 0.1f), "0.00"), F(Q(d, 0.5f), "0.00"), F(Q(d, 0.9f), "0.00"),
            F(ba, "0.00"), F(Sdi(trees), "0"), F(vol, "0.0"), F(hd.Average(), "0.0"), F(Q(hd, 0.9f), "0.0"), F(topHd, "0.0"),
            F(ci.Average(), "0.00"), F(Q(ci, 0.9f), "0.00"), deadThisRun, F(deadwoodM3ThisRun / PlotHectares, "0.0"));
    }

    private const string StandHeader = "run,age,trees,stems_ha,recruits,top_height_m,dominant_height_m,mean_height_m,h_p10,h_p50,h_p90,"
        + "mean_dbh_cm,dq_cm,dbh_p10,dbh_p50,dbh_p90,basal_area_m2_ha,sdi_ha,volume_m3_ha,hd_mean,hd_p90,hd_top,ci_mean,ci_p90,"
        + "cumulative_deaths,cumulative_deadwood_m3_ha";

    private void TreeRows(StreamWriter w, string label, int age, ForestTree[] trees)
    {
        foreach (ForestTree t in trees)
            w.WriteLine(string.Join(",", label, age, t.TreeId, t.AgeYears, F(t.Diameter, "0.000"), F(t.Height, "0.000"),
                F(e.GetCompetitionIndex(t), "0.000"), F(t.CrownRadius, "0.000"), F(t.BiologicalStemVolumeM3, "0.0000"),
                F(e.GetAnnualDbhGrowth(t), "0.0000"), F(t.EquivalentSuppressedYears, "0.00")));
    }

    // ---------- runs ----------

    private IEnumerator StartingStand(StreamWriter standCsv, StreamWriter treeCsv)
    {
        Build();
        yield return null;
        ForestTree[] trees = Living();
        standCsv.WriteLine(StandLine("start", StartAge, trees));
        TreeRows(treeCsv, "start", StartAge, trees);
        float[] sites = e.Cells.Select(c => c.SiteProductivity).Distinct().ToArray();
        Fact($"START trees={trees.Length} ages=[{string.Join(";", trees.Select(t => t.AgeYears).Distinct())}] siteProductivityValues=[{string.Join(";", sites.Select(s => F(s)))}] "
            + $"sitka potentialHeightGrowth={F(sitka.PotentialHeightGrowthMPerYear)} maxHeight={F(sitka.MaxHeightM)} potentialDbh={F(sitka.PotentialDbhGrowthCmPerYear)} maxDbh={F(sitka.MaxDbhCm)} ci50={F(sitka.Ci50)} formHeightRatio={F(sitka.FormHeightRatio)}");
    }

    private IEnumerator Run(Regime r, Mortality m, string label, StreamWriter standCsv, StreamWriter treeCsv, StreamWriter hashes)
    {
        Build();
        var sw = Stopwatch.StartNew();
        double mortalityMs = 0;
        if (Ages.Contains(StartAge)) { standCsv.WriteLine(StandLine(label, StartAge, Living())); TreeRows(treeCsv, label, StartAge, Living()); }
        for (int y = 1; y <= years; y++)
        {
            int age = StartAge + y;
            if (r.Ages.Contains(age - 1)) Thin(r); // thinning at the start of the year reaching that age
            e.AdvanceOneYear();
            var msw = Stopwatch.StartNew();
            ApplyMortality(m);
            mortalityMs += msw.Elapsed.TotalMilliseconds;
            if (Ages.Contains(age))
            {
                ForestTree[] trees = Living();
                standCsv.WriteLine(StandLine(label, age, trees));
                TreeRows(treeCsv, label, age, trees);
                standCsv.Flush(); treeCsv.Flush();
            }
            if (y % 10 == 0) { yield return Resources.UnloadUnusedAssets(); GC.Collect(); }
            else yield return null;
        }
        string state = string.Join("\n", Living().Select(t => t.TreeId + "|" + F(t.Diameter, "R") + "|" + F(t.Height, "R")));
        using (SHA256 sha = SHA256.Create())
            hashes.WriteLine(label + "," + BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(state))).Replace("-", "").Substring(0, 16));
        hashes.Flush();
        Fact($"RUN {label} years={years} deaths={deadThisRun} deadwood_m3_ha={F(deadwoodM3ThisRun / PlotHectares, "0.0")} seconds={F(sw.Elapsed.TotalSeconds, "0.0")} mortalityPassMsTotal={F(mortalityMs, "0.0")}");
    }

    private IEnumerator Performance(StreamWriter perf)
    {
        foreach (int n in new[] { 336, 1300, 3000, 5000 })
        {
            foreach (ForestTree t in FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                DestroyImmediate(t.gameObject);
            e.ResetForDeterministicRun();
            e.RngModelVersion = SimulationRandom.MixedModel;
            e.RegenerationModelVersion = RegenerationModel.AgeBands;
            int side = Mathf.CeilToInt(Mathf.Sqrt(n));
            float spacing = 39f / side;
            int made = 0;
            for (int i = 0; i < side && made < n; i++)
                for (int j = 0; j < side && made < n; j++, made++)
                {
                    float dbh = 12f + (made * 7919 % 100) / 10f;
                    spawner.Spawn("PERF" + made.ToString("00000"), sitka, new Vector3(-19.5f + i * spacing, 0f, -19.5f + j * spacing), 30, dbh, 2f + 0.62f * dbh, sitka.PotentialCrownRadiusM(dbh));
                }
            e.InvalidateCompetition();
            yield return null;
            double stepMs = 0, aMs = 0, cMs = 0;
            const int reps = 3;
            for (int k = 0; k < reps; k++)
            {
                var sw = Stopwatch.StartNew(); e.AdvanceOneYear(); stepMs += sw.Elapsed.TotalMilliseconds;
                // Cost only: evaluate the hazard / SDI without killing.
                sw.Restart();
                ForestTree[] living = Living();
                int risky = 0;
                foreach (ForestTree t in living)
                {
                    float stress = Mathf.Clamp01((Suppression(t) - SuppressionOnset) / (1f - SuppressionOnset));
                    if (SimulationRandom.Roll(1, "MORT-" + t.TreeId, e.EcologicalYear, e.SimulationSeed) < 0.1f * stress * stress) risky++;
                }
                aMs += sw.Elapsed.TotalMilliseconds;
                sw.Restart();
                float sdi = Sdi(living);
                var ordered = living.OrderByDescending(Suppression).ThenBy(t => t.TreeId, StringComparer.Ordinal).ToArray();
                cMs += sw.Elapsed.TotalMilliseconds;
                yield return null;
            }
            perf.WriteLine(string.Join(",", n, Living().Length, F(stepMs / reps, "0.0"), F(aMs / reps, "0.00"), F(cMs / reps, "0.00")));
            perf.Flush();
        }
    }

    // ---------- timber impact of height candidates (production economy) ----------

    private static float ClassEnvelope(string cls, float age)
    {
        float b2, b3, a30;
        switch (cls)
        {
            case "II": b2 = 0.025f; b3 = 1.247f; a30 = 23.3f; break;
            case "IV": b2 = 0.051f; b3 = 2.134f; a30 = 17.4f; break;
            default: b2 = 0.042f; b3 = 1.563f; a30 = 20.4f; break;
        }
        double b0 = a30 / Math.Pow(1 - Math.Exp(-b2 * 30), b3);
        return (float)(b0 * Math.Pow(1 - Math.Exp(-b2 * age), b3));
    }

    // Candidate height for a planted tree from its authored start height.
    private static float CandidateHeight(string candidate, float startHeight, int age)
    {
        switch (candidate)
        {
            case "A_irish_CR_class_III": return startHeight * ClassEnvelope("III", age) / ClassEnvelope("III", StartAge);
            case "A_irish_CR_class_II": return startHeight * ClassEnvelope("II", age) / ClassEnvelope("II", StartAge);
            case "A_irish_CR_class_IV": return startHeight * ClassEnvelope("IV", age) / ClassEnvelope("IV", StartAge);
            case "C_retuned_mitscherlich_III":
                const double g = 1.1886, hmax = 34.36;
                return (float)(hmax - (hmax - startHeight) * Math.Exp(-g / hmax * (age - StartAge)));
            default: return float.NaN;
        }
    }

    private IEnumerator TimberImpact(StreamWriter w)
    {
        string[] candidates = { "production_current", "A_irish_CR_class_III", "C_retuned_mitscherlich_III", "A_irish_CR_class_II", "A_irish_CR_class_IV" };
        int[] standAges = { 21, 36, 45, 60 };
        Build();
        var startHeights = Living().ToDictionary(t => t.TreeId, t => t.Height);
        int age = StartAge;
        foreach (int target in standAges)
        {
            while (age < target) { e.AdvanceOneYear(); age++; yield return null; }
            ForestTree[] living = Living().Where(t => t.CanChop).ToArray();
            var sets = new Dictionary<string, ForestTree[]>
            {
                ["below30pct"] = living.OrderBy(t => t.Diameter).ThenBy(t => t.TreeId, StringComparer.Ordinal).Take(Mathf.RoundToInt(living.Length * 0.3f)).ToArray(),
                ["crowded49"] = living.OrderByDescending(t => e.GetCompetitionIndex(t)).ThenBy(t => t.TreeId, StringComparer.Ordinal).Take(49).ToArray(),
            };
            var productionHeights = living.ToDictionary(t => t.TreeId, t => t.Height);
            foreach (var set in sets)
                foreach (string candidate in candidates)
                {
                    foreach (ForestTree t in set.Value)
                    {
                        float h = candidate == "production_current" || !startHeights.ContainsKey(t.TreeId)
                            ? productionHeights[t.TreeId] : CandidateHeight(candidate, startHeights[t.TreeId], age);
                        t.ApplyGrowth(0f, h - t.Height);
                    }
                    int id = 1;
                    var orders = set.Value.Select(t => new ScenarioOneWorkOrder { workOrderId = id++, type = ScenarioWorkType.FellTree, targetTreeId = t.TreeId,
                        executionMethod = CCF.Forestry.WorkEconomy.WorkExecutionMethod.Contractor, fellingOutcome = FellingMaterialOutcome.SellAndExtract }).ToList();
                    ScenarioHarvestJob job = ScenarioOneEconomyAdapter.QuoteHarvest(orders, set.Value.ToDictionary(t => t.TreeId), manager.Definition, age - StartAge, 1, 100000000L);
                    string assortments = job.Yield == null ? "" : string.Join(";", job.Yield.Assortments.Where(a => a.VolumeCm3 > 0)
                        .GroupBy(a => a.Assortment).Select(g => g.Key + "=" + F(g.Sum(a => a.VolumeCm3) / 1e6, "0.000")));
                    w.WriteLine(string.Join(",", age, age - StartAge, set.Key, candidate, set.Value.Length,
                        F(set.Value.Average(t => t.Height), "0.00"), F(set.Value.Average(t => t.Diameter), "0.00"),
                        F(job.TotalStemVolumeCm3 / 1e6, "0.000"), F(job.SoldVolumeCm3 / 1e6, "0.000"), job.RevenueCents, job.CostCents,
                        job.RevenueCents - job.CostCents, "\"" + assortments + "\"", "\"" + (job.Problem ?? "") + "\""));
                    foreach (ForestTree t in set.Value) t.ApplyGrowth(0f, productionHeights[t.TreeId] - t.Height);
                }
            w.Flush();
        }
    }

    private IEnumerator Execute()
    {
        e = FindFirstObjectByType<ForestEcologyController>();
        stand = FindFirstObjectByType<ForestStartingStand>();
        spawner = FindFirstObjectByType<ForestTreeSpawner>();
        manager = FindFirstObjectByType<ScenarioOneManager>();
        sitka = spawner.ResolveSpecies("sitka-spruce");
        dir = Environment.GetEnvironmentVariable("CCF_DIAG_DIR") ?? Path.Combine(Application.temporaryCachePath, "sitka-growth");
        years = int.TryParse(Environment.GetEnvironmentVariable("CCF_SITKA_YEARS"), out int parsed) ? parsed : 100;
        Directory.CreateDirectory(dir);
        using (facts = new StreamWriter(Path.Combine(dir, "facts.txt")))
        using (var standCsv = new StreamWriter(Path.Combine(dir, "stand_development.csv")))
        using (var treeCsv = new StreamWriter(Path.Combine(dir, "tree_snapshots.csv")))
        using (var hashes = new StreamWriter(Path.Combine(dir, "run_hashes.csv")))
        using (var perf = new StreamWriter(Path.Combine(dir, "performance.csv")))
        {
            standCsv.WriteLine(StandHeader);
            treeCsv.WriteLine("run,stand_age,tree_id,tree_age,dbh_cm,height_m,ci,crown_radius_m,volume_m3,dbh_growth_cm,equivalent_suppressed_years");
            hashes.WriteLine("run,state_hash");
            perf.WriteLine("requested_trees,living_trees,annual_step_ms,suppression_hazard_pass_ms,sdi_pass_ms");
            Fact($"ENV engine={Application.unityVersion} years={years} rngModel=1 regenerationModel=1 browsePressure={F(manager.Definition.BackgroundBrowsePressure)}");
            if (Environment.GetEnvironmentVariable("CCF_SITKA_MODE") == "timber")
            {
                using (var timber = new StreamWriter(Path.Combine(dir, "timber_impact.csv")))
                {
                    timber.WriteLine("stand_age,scenario_year,thinning_set,candidate,trees,mean_height_m,mean_dbh_cm,stem_volume_m3,sold_volume_m3,revenue_cents,cost_cents,net_cents,assortments_m3,problem");
                    yield return TimberImpact(timber);
                }
                Fact("SITKA_TIMBER_DONE");
                yield break;
            }
            yield return StartingStand(standCsv, treeCsv);
            foreach (Mortality m in Mortalities)
                foreach (Regime r in Regimes)
                    yield return Run(r, m, m.Name + "/" + r.Name, standCsv, treeCsv, hashes);
            // Sensitivity of the suppression hazard (unthinned) and determinism repeats.
            yield return Run(Regimes[0], new Mortality { Name = "A_low", HazardMax = 0.05f, Suppression = true }, "A_suppression_hazard0.05/unthinned", standCsv, treeCsv, hashes);
            yield return Run(Regimes[0], new Mortality { Name = "A_high", HazardMax = 0.20f, Suppression = true }, "A_suppression_hazard0.20/unthinned", standCsv, treeCsv, hashes);
            yield return Run(Regimes[0], new Mortality { Name = "C_low", SdiMax = 1500f }, "C_self_thinning_sdi1500/unthinned", standCsv, treeCsv, hashes);
            yield return Run(Regimes[0], Mortalities[0], "repeat:E_none/unthinned", standCsv, treeCsv, hashes);
            yield return Run(Regimes[0], Mortalities[1], "repeat:A_suppression/unthinned", standCsv, treeCsv, hashes);
            yield return Performance(perf);
            Fact("SITKA_DIAGNOSTICS_DONE");
        }
    }

    private IEnumerator Start()
    {
        yield return null; yield return null;
        string failure = null;
        var stack = new Stack<IEnumerator>();
        stack.Push(Execute());
        while (stack.Count > 0)
        {
            bool more = false; object current = null;
            try { more = stack.Peek().MoveNext(); if (more) current = stack.Peek().Current; }
            catch (Exception ex) { failure = ex.ToString(); }
            if (failure != null) break;
            if (!more) { stack.Pop(); continue; }
            if (current is IEnumerator nested) { stack.Push(nested); continue; }
            yield return current;
        }
        Debug.Log(failure == null ? "SITKA_DIAGNOSTICS_PASS" : "SITKA_DIAGNOSTICS_FAIL " + failure);
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(failure == null ? 0 : 1);
#endif
    }
}
