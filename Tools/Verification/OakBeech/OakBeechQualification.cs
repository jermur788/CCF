#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using CCF.Forestry.WorkEconomy;

// Disposable Oak/Beech ordinal [C] production-calibration qualification gate (growth rows 796da99).
//
// Phases (env OBQ_PHASE):
//   rows     runtime parameters (2A) and controlled Oak/Beech trajectories by competition regime (2B)
//   release  matched stands before/after a legitimate competition release (2D)
//   cohort   Scenario One cohorts planted through the normal game APIs, measured +10..+50 y (2C), old-vs-new rows, in-process save/load (2E)
//   save1    cohort -> Y20 -> real file save (2E, relaunch part 1)
//   save2    relaunched process: load, verify species rows / cohort origin / continuation hash (2E part 2)
//
// The kernel is checked against the closed form the controller documents (DBH: Pd x site x (1 - D/Dmax) / (1 + CI/Ci50);
// height: Ph x site x (1 - H/Hmax)), using the competition index actually measured before each annual step, so the check
// does not depend on any particular competition layout. Controlled fixtures plant real juveniles through the manager,
// switch browsing off, and delete natural recruits each year (documented fixture controls, not game behaviour).
// Saves go only to Application.persistentDataPath, which the runner isolates with XDG_CONFIG_HOME.
public static class OakBeechQualification
{
    public static void Begin()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        EditorApplication.isPlaying = true;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (!Environment.GetCommandLineArgs().Any(a => a == "OakBeechQualification.Begin")) return;
        new GameObject("Oak Beech qualification").AddComponent<OakBeechGate>();
    }
}

public sealed class OakBeechGate : MonoBehaviour
{
    private const string Sitka = "sitka-spruce", Oak = "sessile-oak", Beech = "beech";
    private const string OakItem = "sessile-oak-sapling", BeechItem = "beech-sapling";
    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
    private static readonly int[] Ages = { 5, 10, 20, 25, 30, 40, 50, 75, 100, 125 };

    private struct Rows { public float pd, ph, dmax, hmax, ci50; public Rows(float a, float b, float c, float d, float e) { pd = a; ph = b; dmax = c; hmax = d; ci50 = e; } }
    private static readonly Rows ChosenOak = new Rows(0.8f, 0.65f, 120f, 42.5f, 5f);
    private static readonly Rows ChosenBeech = new Rows(0.9f, 0.75f, 120f, 42.5f, 6.5f);
    private static readonly Rows SitkaRow = new Rows(1.2f, 0.45f, 120f, 35f, 5f);
    private static readonly Rows PreCalibration = new Rows(0.4f, 0.4f, 120f, 40f, 3f);

    private ForestEcologyController e;
    private ScenarioOneManager manager;
    private ForestTreeSpawner spawner;
    private ForestSaveController saves;
    private ForestTreeMarkingManager marking;
    private ForestSaveData original;
    private string phase, outDir;
    private readonly List<string> failures = new List<string>();
    private readonly StringBuilder evidence = new StringBuilder();
    private int checks;

    // ---------- reporting ----------
    private void Expect(bool ok, string what)
    {
        checks++;
        string line = $"OBQ_{(ok ? "PASS" : "FAIL")} {what}";
        Debug.Log(line); evidence.AppendLine(line);
        if (!ok) failures.Add(what);
    }
    private void Note(string key, string value)
    {
        string line = $"OBQ_NOTE {key} {value}";
        Debug.Log(line); evidence.AppendLine(line);
    }
    private static void Need(bool ok, string what) { if (!ok) throw new InvalidOperationException("SETUP: " + what); }
    private static string F(double v, string f = "0.###") => v.ToString(f, Inv);
    private int Year => e.EcologicalYear;

    // ---------- driver ----------
    private IEnumerator Start()
    {
        yield return null; yield return null;
        e = FindFirstObjectByType<ForestEcologyController>();
        manager = FindFirstObjectByType<ScenarioOneManager>();
        spawner = FindFirstObjectByType<ForestTreeSpawner>();
        saves = FindFirstObjectByType<ForestSaveController>();
        marking = FindFirstObjectByType<ForestTreeMarkingManager>();
        phase = Environment.GetEnvironmentVariable("OBQ_PHASE") ?? "rows";
        outDir = Environment.GetEnvironmentVariable("OBQ_OUT") ?? Path.Combine(Application.dataPath, "..", "Build", "OBQ");
        Directory.CreateDirectory(outDir);
        Exception failure = null;
        var stack = new Stack<IEnumerator>();
        try
        {
            Need(e != null && manager != null && spawner != null && saves != null && marking != null, "scene systems");
            original = saves.CaptureData();
            Note("phase", phase);
            Note("head", $"geometry={StandGeometryModel.Label(e.StandGeometryModelVersion)} growthModel={e.GrowthModelVersion} rngModel={e.RngModelVersion} regenModel={e.RegenerationModelVersion} saveVersion={ForestSaveData.CurrentVersion}");
            stack.Push(Run());
        }
        catch (Exception ex) { failure = ex; }
        while (failure == null && stack.Count > 0)
        {
            bool more; object current = null;
            try { more = stack.Peek().MoveNext(); if (more) current = stack.Peek().Current; }
            catch (Exception ex) { failure = ex; break; }
            if (!more) { stack.Pop(); continue; }
            if (current is IEnumerator nested) stack.Push(nested); else yield return current;
        }
        try { RestoreRows(); } catch (Exception ex) { failure = failure ?? ex; }
        bool ok = failure == null && failures.Count == 0;
        if (failure != null) { Debug.LogError("OBQ_VERIFY_ABORT " + failure); evidence.AppendLine("OBQ_VERIFY_ABORT " + failure); }
        evidence.AppendLine((ok ? "OBQ_VERIFY_PASS " : "OBQ_VERIFY_FAIL ") + $"phase={phase} checks={checks} failed={failures.Count}");
        try { File.WriteAllText(Path.Combine(outDir, "evidence-" + phase + (Environment.GetEnvironmentVariable("OBQ_TAG") ?? "") + ".txt"), evidence.ToString()); } catch (Exception ex) { Debug.LogError(ex); }
        Debug.Log(ok ? $"OBQ_VERIFY_PASS phase={phase} checks={checks}" : $"OBQ_VERIFY_FAIL phase={phase} failed=[{string.Join(" | ", failures)}] abort={failure}");
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(ok ? 0 : 1);
    }

    private IEnumerator Run()
    {
        switch (phase)
        {
            case "rows": yield return RowsPhase(); break;
            case "release": yield return ReleasePhase(); break;
            case "cohort": yield return CohortPhase(false); break;
            case "save1": yield return CohortPhase(true); break;
            case "save2": yield return Save2(); break;
            default: throw new InvalidOperationException("unknown OBQ_PHASE " + phase);
        }
    }

    // ---------- species rows ----------
    private static readonly string[] RowFields = { "potentialDbhGrowthCmPerYear", "potentialHeightGrowthMPerYear", "maxDbhCm", "maxHeightM", "ci50" };
    private readonly Dictionary<string, Rows> savedRows = new Dictionary<string, Rows>();

    private static Rows ReadRows(TreeSpeciesDefinition s) => new Rows(s.PotentialDbhGrowthCmPerYear, s.PotentialHeightGrowthMPerYear, s.MaxDbhCm, s.MaxHeightM, s.Ci50);

    private static void WriteRows(TreeSpeciesDefinition s, Rows r)
    {
        // In-memory only: no SetDirty, no AssetDatabase.SaveAssets, so the asset on disk is untouched.
        float[] values = { r.pd, r.ph, r.dmax, r.hmax, r.ci50 };
        for (int i = 0; i < RowFields.Length; i++)
            typeof(TreeSpeciesDefinition).GetField(RowFields[i], BindingFlags.Instance | BindingFlags.NonPublic).SetValue(s, values[i]);
    }

    private void UseRows(string speciesId, Rows r)
    {
        TreeSpeciesDefinition s = spawner.ResolveSpecies(speciesId);
        if (!savedRows.ContainsKey(speciesId)) savedRows[speciesId] = ReadRows(s);
        WriteRows(s, r);
    }

    private void RestoreRows()
    {
        foreach (var kv in savedRows) WriteRows(spawner.ResolveSpecies(kv.Key), kv.Value);
        savedRows.Clear();
    }

    private static bool Same(Rows a, Rows b) => a.pd == b.pd && a.ph == b.ph && a.dmax == b.dmax && a.hmax == b.hmax && a.ci50 == b.ci50;
    private static string Fmt(Rows r) => $"Pd={F(r.pd)} Ph={F(r.ph)} Dmax={F(r.dmax)} Hmax={F(r.hmax)} Ci50={F(r.ci50)}";

    // ---------- fixtures ----------
    private sealed class Row { public int year; public float d, h, ci; }
    private sealed class Fix
    {
        public string regime, juvId, treeId; public int ix, iz; public Vector3 pos; public int promotedYear = -1; public float promotedD, promotedH;
        public readonly List<Row> rows = new List<Row>();
        public bool hadPre; public float preD, preH, preCi, preSite;
        public bool Centre => ix == 2 && iz == 2;
        public bool Inner => ix >= 1 && ix <= 3 && iz >= 1 && iz <= 3;
    }
    private List<Fix> fix = new List<Fix>();
    private float maxLawD, maxLawH; private int lawChecks, deaths;
    private Dictionary<string, ForestTree> treeMap = new Dictionary<string, ForestTree>();

    private IEnumerator MakeWorld(int growthModel)
    {
        foreach (ForestTree t in FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None)) DestroyImmediate(t.gameObject);
        manager.InitializeNewScenario();
        e.ResetForDeterministicRun();
        e.RngModelVersion = SimulationRandom.MixedModel;
        e.RegenerationModelVersion = RegenerationModel.AgeBands;
        e.GrowthModelVersion = growthModel;
        e.Browsing.BackgroundPressure = 0f;   // fixture control: zero-browse surviving stock (the lab's "stock" control)
        e.InvalidateCompetition();
        e.RecomputeCanopy();
        fix = new List<Fix>(); maxLawD = maxLawH = 0f; lawChecks = deaths = 0;
        yield return null;
    }

    private static IEnumerable<(int ix, int iz, Vector3 pos)> Grid(float cx, float cz, float spacing)
    {
        for (int i = 0; i < 5; i++) for (int j = 0; j < 5; j++) yield return (i, j, new Vector3(cx + (i - 2) * spacing, 0f, cz + (j - 2) * spacing));
    }

    private void PlantGrids(string itemId, params (string regime, float cx, float spacing)[] grids)
    {
        int n = grids.Length * 25;
        Need(manager.TryPurchaseStock(itemId, n), "stock: " + manager.Feedback);
        foreach (var g in grids)
            foreach (var (ix, iz, pos) in Grid(g.cx, 0f, g.spacing))
            {
                Need(manager.TryDesignateExactPlanting(itemId, pos, WorkExecutionMethod.Contractor, false), $"designate {g.regime} {ix},{iz}: {manager.Feedback}");
                fix.Add(new Fix { regime = g.regime, ix = ix, iz = iz, pos = pos });
            }
        Need(manager.ApprovePendingWork(), "approve planting: " + manager.Feedback);
        Need(manager.AdvanceYear() && Year == 1, "planting resolves in year 1: " + manager.Feedback);
        foreach (Fix f in fix)
        {
            PlantedJuvenile j = manager.PlantedJuveniles.FirstOrDefault(p => Vector2.Distance(new Vector2(p.position.x, p.position.z), new Vector2(f.pos.x, f.pos.z)) < 0.01f);
            Need(j != null, "juvenile at " + f.pos);
            f.juvId = j.juvenileId;
        }
        Need(manager.PlantedJuveniles.Count(p => p.alive) == n, "all saplings planted and alive: " + manager.PlantedJuveniles.Count(p => p.alive) + "/" + n);
    }

    private void RemoveRecruits()
    {
        foreach (ForestTree t in FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            if (!t.TreeId.StartsWith("PL-", StringComparison.Ordinal)) DestroyImmediate(t.gameObject);
    }

    private void BuildMap()
    {
        treeMap = FindObjectsByType<ForestTree>(FindObjectsInactive.Include, FindObjectsSortMode.None).Where(t => t.IsLiving).ToDictionary(t => t.TreeId, t => t);
    }

    private bool JuvenilesPending() => manager.PlantedJuveniles.Any(j => j.alive && string.IsNullOrEmpty(j.promotedTreeId) && !j.legacyCohortManaged);

    private void PreStep()
    {
        BuildMap();
        e.InvalidateCompetition();
        foreach (Fix f in fix)
        {
            f.hadPre = false;
            if (f.treeId == null || !treeMap.TryGetValue(f.treeId, out ForestTree t)) continue;
            f.preD = t.Diameter; f.preH = t.Height; f.preCi = e.GetCompetitionIndex(t); f.preSite = e.GetSiteProductivity(t.transform.position); f.hadPre = true;
        }
    }

    private void PostStep()
    {
        RemoveRecruits();
        BuildMap();
        e.InvalidateCompetition();
        foreach (Fix f in fix)
        {
            if (f.treeId == null)
            {
                string id = "PL-" + f.juvId;
                if (!treeMap.TryGetValue(id, out ForestTree born)) continue;
                f.treeId = id; f.promotedYear = Year; f.promotedD = born.Diameter; f.promotedH = born.Height;
            }
            if (!treeMap.TryGetValue(f.treeId, out ForestTree t)) { if (f.hadPre) deaths++; continue; }
            f.rows.Add(new Row { year = Year, d = t.Diameter, h = t.Height, ci = e.GetCompetitionIndex(t) });
            if (f.hadPre)
            {
                TreeSpeciesDefinition s = t.Species;
                float dd = s.PotentialDbhGrowthCmPerYear * f.preSite * Mathf.Clamp01(1f - f.preD / s.MaxDbhCm) * (1f / (1f + f.preCi / s.Ci50));
                float hh = s.PotentialHeightGrowthMPerYear * f.preSite * Mathf.Clamp01(1f - f.preH / s.MaxHeightM);
                float pd = Mathf.Clamp(f.preD + dd, 1f, 200f), ph = Mathf.Clamp(f.preH + hh, 0.1f, 60f);
                maxLawD = Mathf.Max(maxLawD, Mathf.Abs(pd - t.Diameter)); maxLawH = Mathf.Max(maxLawH, Mathf.Abs(ph - t.Height)); lawChecks++;
            }
        }
    }

    private IEnumerator RunTo(int year)
    {
        while (Year < year)
        {
            PreStep();
            if (JuvenilesPending() && manager.CurrentEcologicalYear < manager.CenturyReviewYear) Need(manager.AdvanceYear(), "manager advance at " + Year + ": " + manager.Feedback);
            else e.AdvanceOneYear();
            PostStep();
            if (Year % 10 == 0) yield return null;
        }
    }

    private Row At(Fix f, int year) => f.rows.FirstOrDefault(r => r.year == year);

    private static string Csv(Fix f, Row r, string species, int model)
        => string.Join(",", species, model, f.regime, f.ix + "-" + f.iz, r.year, F(r.d, "0.0000"), F(r.h, "0.0000"), F(r.ci, "0.0000"), F(r.h > 0 ? r.d / r.h : 0, "0.0000"));

    // ============================================================ ROWS (2A runtime + 2B) ============================================================
    private IEnumerator RowsPhase()
    {
        TreeSpeciesDefinition oak = spawner.ResolveSpecies(Oak), beech = spawner.ResolveSpecies(Beech), sitka = spawner.ResolveSpecies(Sitka);
        Expect(oak != null && beech != null && sitka != null, "species resolve through the spawner");
        Expect(Same(ReadRows(oak), ChosenOak), "2A: Sessile Oak runtime row == chosen [C] row (" + Fmt(ReadRows(oak)) + ")");
        Expect(Same(ReadRows(beech), ChosenBeech), "2A: European Beech runtime row == chosen [C] row (" + Fmt(ReadRows(beech)) + ")");
        Expect(Same(ReadRows(sitka), SitkaRow), "2A: Sitka runtime row unchanged (" + Fmt(ReadRows(sitka)) + ")");
        Expect(!Same(ReadRows(oak), ReadRows(beech)), "Oak and Beech rows are not identical");
        Expect(ForestSaveData.CurrentVersion == 20, "save version 20");
        Expect(GrowthModel.Latest == GrowthModel.SpeciesAware && GrowthModel.SiteClassDensity == 1, "growth-model constants unchanged");
        Note("default_growth_model_new_game", e.GrowthModelVersion.ToString());

        int[] models = (Environment.GetEnvironmentVariable("OBQ_MODELS") ?? "1,2,0").Split(',').Select(int.Parse).ToArray();
        var csv = new StringBuilder("species,model,regime,tree,age,D_cm,H_m,CI,D_over_H\n");
        foreach (string species in new[] { Oak, Beech })
            foreach (int model in models)
            {
                int horizon = model == models[0] ? 125 : 60;
                yield return MakeWorld(model);
                string item = species == Oak ? OakItem : BeechItem;
                PlantGrids(item, ("dense", -28f, 2f), ("moderate", -6f, 4f), ("strong", 24f, 6f));
                yield return RunTo(horizon);
                string tag = $"{species}/m{model}";
                Note("lawcheck_" + tag.Replace('/', '_'), $"treeYears={lawChecks} maxDeltaD={maxLawD:E2} maxDeltaH={maxLawH:E2} deaths={deaths} fixtureTrees={fix.Count} living={fix.Count(f => f.rows.Count > 0 && f.rows.Last().year == Year)}");
                Expect(lawChecks > 0 && maxLawD < 1e-3f && maxLawH < 1e-3f, $"2B {tag}: every tree-year follows the documented kernel with the measured CI (max |dD|={maxLawD:E2} cm, |dH|={maxLawH:E2} m over {lawChecks} tree-years)");
                foreach (Fix f in fix) foreach (Row r in f.rows) csv.AppendLine(Csv(f, r, species, model));

                Fix Centre(string regime) => fix.Single(f => f.regime == regime && f.Centre);
                Fix dense = Centre("dense"), moderate = Centre("moderate"), strong = Centre("strong");
                string line = "";
                foreach (int age in Ages.Where(a => a <= horizon))
                {
                    Row a = At(dense, age), b = At(moderate, age), c = At(strong, age);
                    if (a == null || b == null || c == null) continue;
                    line += $" | age{age} D {F(a.d, "0.0")}/{F(b.d, "0.0")}/{F(c.d, "0.0")} H {F(a.h, "0.0")}/{F(b.h, "0.0")}/{F(c.h, "0.0")}";
                }
                Note("trajectory_" + tag.Replace('/', '_') + "_dense/moderate/strong", line);
                bool deathFree = deaths == 0;
                Note("deaths_" + tag.Replace('/', '_'), deaths.ToString());

                // Promotion: the new Scenario One cohort becomes a tree at the lab's age 10 with the lab's state.
                Expect(fix.All(f => f.promotedYear == 10), $"2B {tag}: every sapling is promoted at year 10 (got {string.Join(",", fix.Select(f => f.promotedYear).Distinct())})");
                Expect(Mathf.Abs(dense.promotedD - 5.475f) < 0.05f && Mathf.Abs(dense.promotedH - 3.65f) < 0.05f, $"2B {tag}: promotion state D={F(dense.promotedD, "0.000")} H={F(dense.promotedH, "0.000")} (lab 5.475/3.65)");
                // Release direction and height independence (regimes are the same species, same light, same site).
                foreach (int age in new[] { 20, 30, 40, 50, 60 }.Concat(horizon >= 125 ? new[] { 75, 100, 125 } : new int[0]).Where(a => a <= horizon))
                {
                    Row a = At(dense, age), b = At(moderate, age), c = At(strong, age);
                    if (deathFree) Expect(c.d > b.d + 0.01f && b.d > a.d + 0.01f, $"2B {tag} age {age}: DBH strong {F(c.d)} > moderate {F(b.d)} > dense {F(a.d)} (CI {F(c.ci)}/{F(b.ci)}/{F(a.ci)})");
                    Expect(Mathf.Abs(c.h - a.h) < 1e-3f && Mathf.Abs(b.h - a.h) < 1e-3f, $"2B {tag} age {age}: height identical across regimes (no release multiplier): {F(a.h, "0.000")}/{F(b.h, "0.000")}/{F(c.h, "0.000")}");
                }
                // Monotone, bounded, no early saturation, young/new cohorts small.
                TreeSpeciesDefinition sp = species == Oak ? oak : beech;
                foreach (Fix f in new[] { dense, moderate, strong })
                {
                    bool mono = true, bounded = true;
                    for (int i = 1; i < f.rows.Count; i++) { if (f.rows[i].d < f.rows[i - 1].d - 1e-5f || f.rows[i].h < f.rows[i - 1].h - 1e-5f) mono = false; if (f.rows[i].d > sp.MaxDbhCm + 1e-3f || f.rows[i].h > sp.MaxHeightM + 1e-3f) bounded = false; }
                    Expect(mono && bounded, $"2B {tag} {f.regime}: monotone and within Dmax/Hmax");
                    Row at50 = At(f, 50), at25 = At(f, 25);
                    Expect(at50.d < 0.95f * sp.MaxDbhCm && at50.h < 0.95f * sp.MaxHeightM, $"2B {tag} {f.regime}: no early saturation at 50 y (D {F(at50.d)}/{F(sp.MaxDbhCm)}, H {F(at50.h)}/{F(sp.MaxHeightM)})");
                    Expect(at25.d < 20f, $"2B {tag} {f.regime}: new cohort is still small at 25 y (D {F(at25.d)} cm)");
                    // promotion kink: sizes are continuous, the height RATE steps once from the juvenile rate to the tree law. Report the ratio.
                    Row p10 = At(f, 10), p11 = At(f, 11);
                    if (p10 != null && p11 != null)
                    {
                        float juvenileRate = sp.RegenHeightGrowthMPerYear, treeRate = p11.h - p10.h;
                        Note($"promotion_rates_{tag.Replace('/', '_')}_{f.regime}", $"promoted state D {F(f.promotedD, "0.000")} cm H {F(f.promotedH, "0.000")} m at year {f.promotedYear}; height rate juvenile {F(juvenileRate, "0.000")} -> tree {F(treeRate, "0.000")} m/y (ratio {F(treeRate / Mathf.Max(1e-6f, juvenileRate), "0.00")}); D year10->11 {F(p11.d - p10.d, "0.000")} cm");
                    }
                }
            }
        File.WriteAllText(Path.Combine(outDir, "rows-trajectories.csv"), csv.ToString());
        // Oak and Beech do not behave identically (same regime, same age) -- compare the dense centre trees of the first model.
        var rows = File.ReadAllLines(Path.Combine(outDir, "rows-trajectories.csv")).Skip(1).Select(l => l.Split(',')).Where(c => c[1] == models[0].ToString() && c[2] == "dense" && c[3] == "2-2").ToList();
        foreach (int age in new[] { 20, 50, 100 })
        {
            var o = rows.FirstOrDefault(c => c[0] == Oak && c[4] == age.ToString()); var b = rows.FirstOrDefault(c => c[0] == Beech && c[4] == age.ToString());
            if (o == null || b == null) continue;
            Expect(Math.Abs(float.Parse(o[5], Inv) - float.Parse(b[5], Inv)) > 0.05f && Math.Abs(float.Parse(o[6], Inv) - float.Parse(b[6], Inv)) > 0.05f, $"Oak and Beech differ at age {age}: D {o[5]} vs {b[5]}, H {o[6]} vs {b[6]}");
        }
    }

    // ============================================================ RELEASE (2D) ============================================================
    private IEnumerator ReleasePhase()
    {
        foreach (string species in new[] { Oak, Beech })
        {
            yield return MakeWorld(GrowthModel.SpeciesAware);   // model 2 switches the broadleaf market on, so Work Plan felling is quoted
            string item = species == Oak ? OakItem : BeechItem;
            PlantGrids(item, ("control", -30f, 2f), ("moderate", -10f, 2f), ("strong", 10f, 2f));
            yield return RunTo(50);
            Fix C(string r) => fix.Single(f => f.regime == r && f.Centre);
            Fix control = C("control"), moderate = C("moderate"), strong = C("strong");
            Row a50 = At(control, 50), b50 = At(moderate, 50), c50 = At(strong, 50);
            Expect(Mathf.Abs(a50.d - b50.d) < 1e-4f && Mathf.Abs(a50.d - c50.d) < 1e-4f && Mathf.Abs(a50.h - b50.h) < 1e-4f && Mathf.Abs(a50.h - c50.h) < 1e-4f,
                $"2D {species}: the three matched stands are identical at year 50 (D {F(a50.d)}/{F(b50.d)}/{F(c50.d)}, H {F(a50.h)}/{F(b50.h)}/{F(c50.h)})");
            Note($"release_{species}_before", $"CI at 50: {F(a50.ci)}");
            // Legitimate release through the Work Plan: fell the ring-1 neighbours (moderate) or every neighbour (strong).
            var moderateTargets = fix.Where(f => f.regime == "moderate" && !f.Centre && Math.Abs(f.ix - 2) <= 1 && Math.Abs(f.iz - 2) <= 1).ToList();
            var strongTargets = fix.Where(f => f.regime == "strong" && !f.Centre).ToList();
            BuildMap();
            manager.PlanningFellingOutcome = FellingMaterialOutcome.SellAndExtract;
            foreach (Fix f in moderateTargets.Concat(strongTargets)) marking.Mark(treeMap[f.treeId], TreeMarkType.Fell, false);
            int added = manager.AddMarkedTreesToWorkPlan();
            bool approved = manager.ApprovePendingWork();
            Note($"release_{species}_workplan", $"added={added}/{moderateTargets.Count + strongTargets.Count} approved={approved} feedback={manager.Feedback}");
            PreStep();
            if (approved && added == moderateTargets.Count + strongTargets.Count)
            {
                Need(manager.AdvanceYear(), "release resolution: " + manager.Feedback);
                Note($"release_{species}_route", "Work Plan (commissioned felling)");
            }
            else
            {
                foreach (Fix f in moderateTargets.Concat(strongTargets)) if (treeMap.TryGetValue(f.treeId, out ForestTree t)) t.Fell();
                e.AdvanceOneYear();
                Note($"release_{species}_route", "direct ForestTree.Fell() fallback (Work Plan did not take the stems)");
            }
            PostStep();
            // felled neighbours stop recording (their trees are stumps); the centre trees continue
            yield return RunTo(100);
            Expect(control.rows.Count > 0 && At(control, 100) != null && At(moderate, 100) != null && At(strong, 100) != null, $"2D {species}: all three centre trees alive at year 100");
            foreach (int year in new[] { 55, 60, 75, 100 })
            {
                Row a = At(control, year), b = At(moderate, year), c = At(strong, year);
                Expect(c.d > b.d && b.d > a.d, $"2D {species} year {year}: released DBH strong {F(c.d)} > moderate {F(b.d)} > suppressed {F(a.d)}");
                Expect(Mathf.Abs(a.h - b.h) < 1e-3f && Mathf.Abs(a.h - c.h) < 1e-3f, $"2D {species} year {year}: height identical released vs suppressed ({F(a.h, "0.000")}/{F(b.h, "0.000")}/{F(c.h, "0.000")})");
            }
            Row s51 = At(strong, 51), s50 = At(strong, 50), c51 = At(control, 51);
            Note($"release_{species}_increment", $"year 50->51 dD strong {F(s51.d - s50.d, "0.000")} control {F(c51.d - a50.d, "0.000")} cm; strong CI {F(s50.ci)} -> {F(s51.ci)}");
            Expect(s51.d - s50.d > c51.d - a50.d, $"2D {species}: release raises the DBH increment immediately");
            Note($"release_{species}_deltaD_Y100", $"strong-control {F(At(strong, 100).d - At(control, 100).d, "0.00")} cm, moderate-control {F(At(moderate, 100).d - At(control, 100).d, "0.00")} cm");
            Note("lawcheck_release_" + species, $"treeYears={lawChecks} maxDeltaD={maxLawD:E2} maxDeltaH={maxLawH:E2}");
            Expect(maxLawD < 1e-3f && maxLawH < 1e-3f, $"2D {species}: kernel check holds through the release ({maxLawD:E2}/{maxLawH:E2})");
        }
    }

    // ============================================================ COHORT (2C) + SAVE/LOAD (2E) ============================================================
    private static List<ForestTree> LivingAll() => FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
        .Where(t => t.IsLiving && !t.IsStump).OrderBy(t => t.TreeId, StringComparer.Ordinal).ToList();
    private static float BasalArea(ForestTree t) => Mathf.PI * Mathf.Pow(t.Diameter / 200f, 2f);
    private static List<string> SelectCropTrees(List<ForestTree> trees) => trees
        .GroupBy(t => (Mathf.FloorToInt(t.transform.position.x / 10f), Mathf.FloorToInt(t.transform.position.z / 10f)))
        .Select(g => g.OrderByDescending(t => t.Diameter).ThenBy(t => t.TreeId, StringComparer.Ordinal).First().TreeId).OrderBy(id => id, StringComparer.Ordinal).ToList();
    private static List<ForestTree> SelectThinning(List<ForestTree> trees, List<string> cropIds, float fraction)
    {
        var crop = new HashSet<string>(cropIds);
        List<ForestTree> crops = trees.Where(t => crop.Contains(t.TreeId)).ToList();
        float total = trees.Sum(BasalArea), target = total * fraction, taken = 0f;
        var result = new List<ForestTree>();
        foreach (ForestTree t in trees.Where(t => !crop.Contains(t.TreeId) && t.CanChop)
                     .OrderBy(t => crops.Count == 0 ? 0f : crops.Min(c => Vector3.Distance(c.transform.position, t.transform.position))).ThenBy(t => t.TreeId, StringComparer.Ordinal))
        { if (taken >= target) break; result.Add(t); taken += BasalArea(t); }
        return result;
    }
    private bool TryPlantPair(int cell, string item)
    {
        Vector2 c = e.Cells[cell].Center; Vector3? first = null;
        for (int ix = -2; ix <= 2; ix++) for (int iz = -2; iz <= 2; iz++)
            {
                var p = new Vector3(c.x + ix * 0.9f, 0f, c.y + iz * 0.9f);
                if (e.GetCellIndex(p) != cell) continue;
                if (first.HasValue && Vector3.Distance(first.Value, p) < 0.8f) continue;
                bool shelter = !first.HasValue;
                if (!manager.TryDesignateExactPlanting(item, p, shelter ? WorkExecutionMethod.LandownerSimulated : WorkExecutionMethod.Contractor, shelter)) continue;
                if (!first.HasValue) { first = p; continue; }
                return true;
            }
        return false;
    }

    // The U4/old-harness Plan A: thinning + four Oak and four Beech pairs, designated and approved together (resolves in year 1).
    private void PlanCohort()
    {
        List<ForestTree> living = LivingAll();
        List<string> crop = SelectCropTrees(living);
        foreach (string id in crop) marking.Mark(living.First(t => t.TreeId == id), TreeMarkType.CropTree, false);
        List<ForestTree> fell = SelectThinning(living, crop, 0.27f);
        manager.PlanningFellingOutcome = FellingMaterialOutcome.SellAndExtract;
        foreach (ForestTree t in fell) marking.Mark(t, TreeMarkType.Fell, false);
        Need(manager.AddMarkedTreesToWorkPlan() == fell.Count && fell.Count > 0, "thinning planned");
        Need(manager.TryPurchaseStock(OakItem, 8) && manager.TryPurchaseStock(BeechItem, 8), "stock");
        int[] bright = Enumerable.Range(0, e.CellCount).OrderByDescending(i => e.Cells[i].Light).ThenBy(i => i).ToArray();
        int oak = 0, beech = 0;
        for (int k = 0; k < bright.Length && (oak < 4 || beech < 4); k++)
        { bool o = oak < 4; if (TryPlantPair(bright[k], o ? OakItem : BeechItem)) { if (o) oak++; else beech++; } }
        Need(oak == 4 && beech == 4, $"pairs {oak}/{beech}");
        Need(manager.ApprovePendingWork(), "approve: " + manager.Feedback);
    }

    private string CohortLine(string species)
    {
        var juveniles = manager.PlantedJuveniles.Where(j => j.speciesId == species).ToList();
        var trees = LivingAll().Where(t => t.Species.SpeciesId == species && t.TreeId.StartsWith("PL-", StringComparison.Ordinal)).ToList();
        string sizes = trees.Count == 0 ? "no promoted trees" : $"D mean {F(trees.Average(t => t.Diameter), "0.0")} [{F(trees.Min(t => t.Diameter), "0.0")}-{F(trees.Max(t => t.Diameter), "0.0")}] H mean {F(trees.Average(t => t.Height), "0.0")} [{F(trees.Min(t => t.Height), "0.0")}-{F(trees.Max(t => t.Height), "0.0")}] CI mean {F(trees.Average(t => e.GetCompetitionIndex(t)), "0.0")}";
        return $"planted={juveniles.Count} aliveJuv={juveniles.Count(j => j.alive && string.IsNullOrEmpty(j.promotedTreeId))} promoted={trees.Count} {sizes}";
    }

    private readonly Dictionary<string, string> cohortHash = new Dictionary<string, string>();

    private IEnumerator RunCohort(string label, int model, bool stopAt20, bool captureAt20)
    {
        yield return Restore(JsonUtility.ToJson(original), label + " fresh");
        e.GrowthModelVersion = model;
        Need(Year == 0 && manager.Outcome == ScenarioOneOutcome.Active, "fresh Year 0");
        PlanCohort();
        Need(manager.AdvanceYear() && Year == 1, "cohort planting resolves in year 1");
        var table = new StringBuilder();
        double[] oakD = new double[6], beechD = new double[6];
        for (int plus = 0; plus <= 50; plus++)
        {
            int target = 1 + plus;
            while (Year < target) { Need(manager.AdvanceYear(), $"{label}: advance at {Year}: {manager.Feedback}"); if (Year % 5 == 0) yield return null; }
            if (captureAt20 && Year == 20) { cohortS20 = JsonUtility.ToJson(saves.CaptureData()); cohortHash["Y20"] = ScenarioReferenceArchive.WorldHash(saves.CaptureData()); }
            if (Year == 30) cohortHash["Y30"] = ScenarioReferenceArchive.WorldHash(saves.CaptureData());
            if (stopAt20 && Year == 20) yield break;
            if (plus > 0 && plus % 10 == 0)
            {
                foreach (string sp in new[] { Oak, Beech })
                {
                    Note($"cohort_{label}_{sp}_plus{plus}", CohortLine(sp));
                    foreach (ForestTree t in LivingAll().Where(t => t.Species.SpeciesId == sp && t.TreeId.StartsWith("PL-", StringComparison.Ordinal)))
                    {
                        Need(float.IsFinite(t.Diameter) && float.IsFinite(t.Height) && t.Diameter > 0f && t.Height > 0f, $"{label} {t.TreeId} finite positive size");
                        TreeSpeciesDefinition s = t.Species;
                        if (t.Diameter > s.MaxDbhCm + 1e-3f || t.Height > s.MaxHeightM + 1e-3f) Expect(false, $"{label} {t.TreeId} exceeded Dmax/Hmax");
                    }
                }
            }
        }
        Note($"cohort_{label}_sitka_Y51", $"living Sitka {LivingAll().Count(t => t.Species.SpeciesId == Sitka)}, mean D {F(LivingAll().Where(t => t.Species.SpeciesId == Sitka).Average(t => t.Diameter), "0.0")}");
        Note($"cohort_{label}_hash", $"Y20={(cohortHash.TryGetValue("Y20", out string h20) ? h20 : "-")} Y30={(cohortHash.TryGetValue("Y30", out string h30) ? h30 : "-")} Y51={ScenarioReferenceArchive.WorldHash(saves.CaptureData())}");
    }

    private string cohortS20;

    private IEnumerator Restore(string json, string label)
    {
        Expect(saves.LoadData(JsonUtility.FromJson<ForestSaveData>(json), false), label + ": LoadData accepted");
        yield return null; yield return null;
    }

    private IEnumerator CohortPhase(bool saveAtTwenty)
    {
        TreeSpeciesDefinition oak = spawner.ResolveSpecies(Oak), beech = spawner.ResolveSpecies(Beech);
        Expect(Same(ReadRows(oak), ChosenOak) && Same(ReadRows(beech), ChosenBeech), "2C: runtime rows are the chosen [C] rows");
        if (saveAtTwenty)
        {
            string file = Path.Combine(Application.persistentDataPath, "forest-save.json");
            if (File.Exists(file)) File.Delete(file);
            yield return RunCohort("save1", e.GrowthModelVersion, true, false);
            saves.Save();
            Expect(File.Exists(file), "2E: save file written");
            string json = File.ReadAllText(file);
            Expect(!json.Contains("potentialDbhGrowthCmPerYear") && !json.Contains("maxDbhCm") && !json.Contains("ci50"), "2E: the save carries no species coefficients (nothing to migrate)");
            Expect(json.Contains("\"version\": 20"), "2E: saved version is 20");
            Note("save1_state", $"year={Year} hash={ScenarioReferenceArchive.WorldHash(saves.CaptureData())} plantedJuveniles={manager.PlantedJuveniles.Count} " + string.Join(" | ", manager.PlantedJuveniles.Take(4).Select(j => $"{j.juvenileId}:{j.speciesId}:plantedY{j.plantingYear}:age{F(j.ageYears)}:{j.promotedTreeId}")));
            Note("save1_oak", CohortLine(Oak)); Note("save1_beech", CohortLine(Beech));
            yield break;
        }
        int model = int.Parse(Environment.GetEnvironmentVariable("OBQ_COHORT_MODEL") ?? e.GrowthModelVersion.ToString(), Inv);
        // candidate rows (as shipped)
        yield return RunCohort($"candidate-m{model}", model, false, true);
        string candidateHash30 = cohortHash["Y30"], candidateS20 = cohortS20, candidateHash20 = cohortHash["Y20"];
        Note("candidate_Y30_hash", candidateHash30);
        // premium-size proxy at +50: Oak veneer mid-log 45 cm, lab proxy D60
        float maxOakD = LivingAll().Where(t => t.Species.SpeciesId == Oak && t.TreeId.StartsWith("PL-", StringComparison.Ordinal)).DefaultIfEmpty().Max(t => t == null ? 0f : t.Diameter);
        float maxBeechD = LivingAll().Where(t => t.Species.SpeciesId == Beech && t.TreeId.StartsWith("PL-", StringComparison.Ordinal)).DefaultIfEmpty().Max(t => t == null ? 0f : t.Diameter);
        Note("candidate_largest_planted_DBH_at_plus50", $"oak {F(maxOakD)} cm, beech {F(maxBeechD)} cm");
        Expect(maxOakD > 0f && maxOakD < 45f, $"2C: no planted Oak reaches the 45 cm veneer mid-log size within 50 years of planting ({F(maxOakD)} cm); lab D60 proxy not reached");
        Expect(maxBeechD > 0f, "2C: planted Beech survive to promotion");

        // 2E in-process: reload the year-20 state (real file route) and continue to year 30 -> identical hash
        string file2 = Path.Combine(Application.persistentDataPath, "forest-save.json");
        File.WriteAllText(file2, candidateS20);
        Expect(saves.CanLoad(out string problem), "2E: CanLoad on the year-20 file" + (problem == null ? "" : " (" + problem + ")"));
        saves.Load();
        yield return null; yield return null;
        Expect(Same(ReadRows(oak), ChosenOak) && Same(ReadRows(beech), ChosenBeech), "2E: species rows resolve identically after Load()");
        Expect(Year == 20 && ScenarioReferenceArchive.WorldHash(saves.CaptureData()) == candidateHash20, "2E: loaded world hash equals the saved Year-20 hash");
        var origin = manager.PlantedJuveniles.Where(j => j.speciesId == Oak || j.speciesId == Beech).ToList();
        Expect(origin.Count == 16 && origin.All(j => j.plantingYear == 1), "2E: cohort origin survives (16 saplings, planting year 1)");
        while (Year < 30) Need(manager.AdvanceYear(), "continue after load");
        Expect(ScenarioReferenceArchive.WorldHash(saves.CaptureData()) == candidateHash30, "2E: trajectory continues deterministically (Y30 hash equals the uninterrupted run)");
        File.Delete(file2);

        // old (pre-calibration) rows, identical play, for the size of the change
        foreach (string id in new[] { Oak, Beech }) UseRows(id, PreCalibration);
        Expect(Same(ReadRows(oak), PreCalibration), "comparison run uses the pre-calibration rows (in memory only)");
        yield return RunCohort($"pre-calibration-m{model}", model, false, true);
        string oldS20 = cohortS20;
        // 2E documented behaviour: a save made under the old rows resumes under the new rows (no migration, rows are not saved).
        RestoreRows();
        Expect(Same(ReadRows(oak), ChosenOak), "rows restored to the shipped values");
        yield return Restore(oldS20, "old save under new rows");
        while (Year < 30) Need(manager.AdvanceYear(), "continue old save under new rows");
        string mixedHash30 = ScenarioReferenceArchive.WorldHash(saves.CaptureData());
        Note("old_save_under_new_rows_Y30_hash", $"{mixedHash30} (candidate uninterrupted {candidateHash30}; pre-calibration uninterrupted {cohortHash["Y30"]})");
        Expect(mixedHash30 != cohortHash["Y30"], "2E: an existing Oak/Beech save picks up the new rows on resume (documented: rows are not part of the save)");
        Note("old_save_under_new_rows_matches_candidate", (mixedHash30 == candidateHash30).ToString());
    }

    private IEnumerator Save2()
    {
        TreeSpeciesDefinition oak = spawner.ResolveSpecies(Oak), beech = spawner.ResolveSpecies(Beech);
        Expect(Same(ReadRows(oak), ChosenOak) && Same(ReadRows(beech), ChosenBeech), "2E relaunch: runtime rows are the chosen [C] rows in the new process");
        Expect(saves.CanLoad(out string problem), "2E relaunch: CanLoad" + (problem == null ? "" : " (" + problem + ")"));
        saves.Load();
        yield return null; yield return null;
        Note("save2_loaded", $"year={Year} hash={ScenarioReferenceArchive.WorldHash(saves.CaptureData())} plantedJuveniles={manager.PlantedJuveniles.Count} " + string.Join(" | ", manager.PlantedJuveniles.Take(4).Select(j => $"{j.juvenileId}:{j.speciesId}:plantedY{j.plantingYear}:age{F(j.ageYears)}:{j.promotedTreeId}")));
        Expect(Year == 20, "2E relaunch: year 20 restored");
        var origin = manager.PlantedJuveniles.Where(j => j.speciesId == Oak || j.speciesId == Beech).ToList();
        Expect(origin.Count == 16 && origin.All(j => j.plantingYear == 1), "2E relaunch: cohort origin/year survives");
        string expectLoaded = Environment.GetEnvironmentVariable("OBQ_EXPECT_Y20_HASH");
        if (!string.IsNullOrEmpty(expectLoaded)) Expect(ScenarioReferenceArchive.WorldHash(saves.CaptureData()) == expectLoaded, "2E relaunch: loaded world hash equals the pre-exit hash");
        Note("save2_oak", CohortLine(Oak)); Note("save2_beech", CohortLine(Beech));
        while (Year < 30) Need(manager.AdvanceYear(), "continue after relaunch");
        string hash30 = ScenarioReferenceArchive.WorldHash(saves.CaptureData());
        Note("save2_Y30_hash", hash30);
        string expect = Environment.GetEnvironmentVariable("OBQ_EXPECT_Y30_HASH");
        if (!string.IsNullOrEmpty(expect)) Expect(hash30 == expect, $"2E relaunch: Y30 hash equals the uninterrupted run ({expect})");
    }
}
#endif
