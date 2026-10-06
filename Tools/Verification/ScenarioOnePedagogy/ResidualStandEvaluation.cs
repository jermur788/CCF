#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Disposable, read-only residual-stand evaluation (pedagogy research harness).
// Docs/Research/ScenarioOnePedagogy/ResidualStandEvaluationPrototype.md
//
// Copy into Assets/ForestPrototype, run
//   -batchmode -nographics -executeMethod ResidualStandEvaluation.Begin
// with an isolated XDG_CONFIG_HOME and CCF_ACCEPTANCE_OUTPUT=<dir>, then
// delete the copy and its .meta. It never calls Save(): every candidate
// marking plan is applied to the in-memory world and undone with
// ForestSaveController.LoadData(captured, false). No production variable is
// added; every metric is read from, or recomputed with, existing production
// formulas (Hegyi term, canopy gap product, growth response, wind diagnostic,
// Work Plan harvest quote).
public static class ResidualStandEvaluation
{
    public static void Begin()
    {
        EditorSceneManager.OpenScene("Assets/Scenes/ForestTest.unity");
        EditorApplication.isPlaying = true;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Install()
    {
        if (Environment.GetCommandLineArgs().Any(a => a == "ResidualStandEvaluation.Begin"))
            new GameObject("Disposable residual-stand evaluation").AddComponent<ResidualStandEvaluationRunner>();
    }
}

public sealed class ResidualStandEvaluationRunner : MonoBehaviour
{
    private static readonly CultureInfo Inv = CultureInfo.InvariantCulture;
    private static readonly int[] LongTermYears = { 1, 5, 10, 20 };
    private const int CropTreeReleaseNeighbours = 2;   // T2: top competitors removed per crop tree
    private const int HeavyReleaseNeighbours = 6;      // T3
    private const int ConservativeNeighbours = 1;      // T4

    private ScenarioOneManager manager;
    private ForestEcologyController ecology;
    private ForestSaveController saves;
    private ForestTreeMarkingManager marking;
    private ForestSaveData original;
    private string originalJson;
    private string lastRow, lastHash;
    private string output;
    private readonly StringBuilder json = new StringBuilder();
    private readonly StringBuilder immediateCsv = new StringBuilder();
    private readonly StringBuilder longCsv = new StringBuilder();
    private readonly StringBuilder cropCsv = new StringBuilder();
    private readonly StringBuilder neighbourCsv = new StringBuilder();
    private readonly StringBuilder treeCsv = new StringBuilder();
    private readonly StringBuilder cellCsv = new StringBuilder();

    private static void Check(bool ok, string message) { if (!ok) throw new InvalidOperationException(message); }
    private static string F(float v, string format = "0.0000") => v.ToString(format, Inv);
    private static string F(double v, string format = "0.0000") => v.ToString(format, Inv);

    private IEnumerator Start()
    {
        yield return null; yield return null;
        Exception failure = null;
        // Stack runner: nested IEnumerators run inside the same try/catch.
        foreach (IEnumerator root in new[] { Run(), Cleanup() })
        {
            var steps = new Stack<IEnumerator>();
            steps.Push(root);
            while (steps.Count > 0)
            {
                bool more; object current = null;
                try { more = steps.Peek().MoveNext(); if (more) current = steps.Peek().Current; }
                catch (Exception e) { failure = failure ?? e; break; }
                if (!more) { steps.Pop(); continue; }
                if (current is IEnumerator child) { steps.Push(child); continue; }
                yield return current;
            }
        }
        if (failure == null) Debug.Log("RESIDUAL_STAND_EVALUATION_PASS");
        else Debug.LogError("RESIDUAL_STAND_EVALUATION_FAIL " + failure);
        EditorApplication.ExitPlaymode();
        EditorApplication.Exit(failure == null ? 0 : 1);
    }

    // Fresh copy per restore (LoadData must never alias the captured state),
    // then two frames so trees removed by the load are really destroyed before
    // anything enumerates or captures the world (CaptureData includes inactive
    // trees).
    private IEnumerator Restore(string json)
    {
        Check(saves.LoadData(JsonUtility.FromJson<ForestSaveData>(json), false), "in-memory restore failed");
        yield return null; yield return null;
        ecology.InvalidateCompetition();
    }

    private IEnumerator Cleanup()
    {
        if (saves == null || originalJson == null) yield break;
        yield return Restore(originalJson);
        Check(ScenarioReferenceArchive.WorldHash(saves.CaptureData()) == ScenarioReferenceArchive.WorldHash(original),
            "world not restored after evaluation");
        Debug.Log("RESIDUAL_STAND_WORLD_RESTORED");
    }

    // ------------------------------------------------------------------ run

    private sealed class Treatment
    {
        public string Id, Description;
        public List<string> Remove = new List<string>();
    }

    private IEnumerator Run()
    {
        manager = FindFirstObjectByType<ScenarioOneManager>();
        ecology = FindFirstObjectByType<ForestEcologyController>();
        saves = FindFirstObjectByType<ForestSaveController>();
        marking = FindFirstObjectByType<ForestTreeMarkingManager>();
        Check(manager != null && ecology != null && saves != null && marking != null, "scene systems missing");
        output = Environment.GetEnvironmentVariable("CCF_ACCEPTANCE_OUTPUT");
        if (string.IsNullOrEmpty(output)) output = Path.Combine(Application.temporaryCachePath, "residual-stand");
        Directory.CreateDirectory(output);
        Check(ecology.EcologicalYear == 0 && Living().Count == 336,
            "not a fresh Scenario One Year 0 (use an isolated XDG_CONFIG_HOME so no save is auto-loaded)");
        original = saves.CaptureData();
        originalJson = JsonUtility.ToJson(original);
        Debug.Log($"RESIDUAL_STAND_MODELS rng={ecology.RngModelVersion} regeneration={ecology.RegenerationModelVersion} "
            + $"browse={F(ecology.Browsing.BackgroundPressure, "0.00")} year={ecology.EcologicalYear}");

        immediateCsv.AppendLine("context,treatment,removed_n,removed_ba_m2ha,removed_vol_m3,retained_n,retained_ba_m2ha,retained_vol_m3,"
            + "gross_revenue_eur,external_cost_eur,minimum_adjustment_eur,net_eur,retained_gross_value_eur,"
            + "crop_n,crop_ci_before,crop_ci_after,crop_ci_change_pct,crop_growth_before_cm,crop_growth_after_cm,crop_growth_change_pct,"
            + "stand_growth_change_pct,removals_near_no_crop,removals_low_effect,release_per_m2ba,"
            + "cells_touched,max_removed_per_cell,clark_evans_R,mean_light_after,max_light_after,cells_light_ge_0_20,"
            + "largest_opened_cluster_cells,light_cv_after,wind_peak_after,retained_high_wind,crop_mean_hd,seed_trees_retained,seed_trees_removed");
        longCsv.AppendLine("treatment,year,living,ba_m2ha,standing_vol_m3,crop_alive,crop_mean_dbh_cm,crop_mean_increment_cm,crop_mean_ci,"
            + "stand_mean_increment_cm,mean_canopy,mean_light,regeneration_cells,max_wind,cash_eur");
        cropCsv.AppendLine("context,treatment,crop_tree,dbh_cm,height_m,ci_before,ci_after,competitors_removed,growth_before_cm,growth_after_cm");
        neighbourCsv.AppendLine("context,crop_tree,crop_dbh_cm,crop_height_m,crop_ci,neighbour,neighbour_dbh_cm,neighbour_height_m,distance_m,hegyi_term,"
            + "share_of_crop_ci,rank,neighbour_competition_label,neighbour_is_crop,removed_in");

        treeCsv.AppendLine("context,tree,species,x_m,z_m,cell,age,dbh_cm,height_m,crown_radius_m,hd_ratio,stem_vol_m3,ci,competition_label,"
            + "suppression,wind_risk,wind_label,seed_bearing,is_crop");
        cellCsv.AppendLine("context,cell,label,centre_x,centre_z,light,canopy,recent_opening,stems,ba_m2ha,regen_species,regen_density,regen_max_height_m");
        json.Append("{\n  \"harness\": \"ResidualStandEvaluation\",\n");
        json.Append($"  \"rngModel\": {ecology.RngModelVersion},\n  \"regenerationModel\": {ecology.RegenerationModelVersion},\n");
        json.Append($"  \"browsePressure\": {F(ecology.Browsing.BackgroundPressure, "0.00")},\n");

        // ---- Year 0: crop-tree framework and treatments ----
        List<ForestTree> living = Living();
        List<string> crops = SelectCropTrees(living);
        json.Append($"  \"cropTrees\": [{string.Join(", ", crops.Select(c => "\"" + c + "\""))}],\n");
        json.Append("  \"baseline\": ").Append(StandSummary(living)).Append(",\n");
        List<Treatment> treatments = BuildTreatments(living, crops);
        NeighbourTable("Y0", living, crops, treatments);
        Inventory("Y0", living, crops);
        json.Append("  \"treatments\": [\n");
        for (int i = 0; i < treatments.Count; i++)
        {
            Treatment t = treatments[i];
            json.Append($"    {{\"id\": \"{t.Id}\", \"description\": \"{t.Description}\", \"removed\": [{string.Join(", ", t.Remove.Select(r => "\"" + r + "\""))}]}}");
            json.Append(i < treatments.Count - 1 ? ",\n" : "\n");
        }
        json.Append("  ],\n  \"immediate\": [\n");
        bool first = true;
        foreach (Treatment t in treatments)
        {
            yield return Immediate("Y0", t, crops, originalJson);
            json.Append(first ? "" : ",\n").Append("    ").Append(lastRow);
            first = false;
        }
        json.Append("\n  ],\n");

        // ---- Long-term: apply through the real work cycle, then advance ----
        json.Append("  \"longTerm\": [\n");
        first = true;
        foreach (Treatment t in treatments)
        {
            yield return Restore(originalJson);
            ApplyThroughWorkPlan(t, crops);
            int target = 0;
            foreach (int year in LongTermYears)
            {
                while (ecology.EcologicalYear < year)
                {
                    Check(manager.AdvanceYear(), $"advance failed for {t.Id} at {ecology.EcologicalYear}: {manager.Feedback}");
                    target++;
                    if (target % 5 == 0) yield return null;
                }
                string row = LongTermRow(t.Id, crops);
                json.Append(first ? "" : ",\n").Append("    ").Append(row);
                first = false;
            }
        }
        json.Append("\n  ],\n");

        // ---- Determinism: repeat T2 to year 20 and compare world hashes ----
        yield return RunToYearHash(treatments.First(t => t.Id == "T2"), crops, 20);
        string hashA = lastHash;
        yield return RunToYearHash(treatments.First(t => t.Id == "T2"), crops, 20);
        string hashB = lastHash;
        Check(hashA == hashB, $"T2 long-term run is not deterministic in-process: {hashA} vs {hashB}");
        json.Append($"  \"determinism\": {{\"T2_year20_world_hash_a\": \"{hashA}\", \"T2_year20_world_hash_b\": \"{hashB}\"}},\n");
        Debug.Log($"RESIDUAL_STAND_DETERMINISM T2Y20 {hashA} {hashB}");

        // ---- Later intervention: untreated stand at Year 10 (trees age 30) ----
        yield return Restore(originalJson);
        while (ecology.EcologicalYear < 10) Check(manager.AdvanceYear(), "untreated advance to Y10 failed");
        yield return null; yield return null;
        string year10 = JsonUtility.ToJson(saves.CaptureData());
        List<ForestTree> living10 = Living();
        List<string> crops10 = SelectCropTrees(living10);
        List<Treatment> later = BuildTreatments(living10, crops10).Where(t => t.Id == "T0" || t.Id == "T2" || t.Id == "T5").ToList();
        NeighbourTable("Y10", living10, crops10, later);
        Inventory("Y10", living10, crops10);
        json.Append("  \"laterIntervention\": {\"year\": 10, \"baseline\": ").Append(StandSummary(living10)).Append(", \"immediate\": [\n");
        first = true;
        foreach (Treatment t in later)
        {
            yield return Immediate("Y10", t, crops10, year10);
            json.Append(first ? "" : ",\n").Append("    ").Append(lastRow);
            first = false;
        }
        json.Append("\n  ]}\n}\n");

        WriteOutputs();
    }

    // ------------------------------------------------------------- selection

    private static List<ForestTree> Living() => FindObjectsByType<ForestTree>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
        .Where(t => t.IsLiving && !t.IsStump).OrderBy(t => t.TreeId, StringComparer.Ordinal).ToList();

    // Same proxy as ScenarioOneCompletionVerification: largest DBH per 10 m block.
    // The game has no stem-form or quality state, so this is a vigour proxy only.
    private static List<string> SelectCropTrees(List<ForestTree> trees) => trees
        .GroupBy(t => (Mathf.FloorToInt(t.transform.position.x / 10f), Mathf.FloorToInt(t.transform.position.z / 10f)))
        .Select(g => g.OrderByDescending(t => t.Diameter).ThenBy(t => t.TreeId, StringComparer.Ordinal).First().TreeId)
        .OrderBy(id => id, StringComparer.Ordinal).ToList();

    private static float Distance(ForestTree a, ForestTree b) => Vector2.Distance(
        new Vector2(a.transform.position.x, a.transform.position.z), new Vector2(b.transform.position.x, b.transform.position.z));

    // Production Hegyi term of `neighbour` on `target` (0 beyond the cutoff).
    private static float Term(ForestTree neighbour, ForestTree target)
    {
        float d = Distance(neighbour, target);
        return d > ForestEcologyController.HegyiCutoffMeters ? 0f
            : ForestEcologyController.HegyiTerm(neighbour.Diameter, target.Diameter, d);
    }

    private List<Treatment> BuildTreatments(List<ForestTree> living, List<string> cropIds)
    {
        var crop = new HashSet<string>(cropIds);
        List<ForestTree> crops = living.Where(t => crop.Contains(t.TreeId)).ToList();
        List<ForestTree> candidates = living.Where(t => !crop.Contains(t.TreeId) && t.CanChop).ToList();
        var list = new List<Treatment> { new Treatment { Id = "T0", Description = "no treatment" } };

        // T1 novice clean-up: every crowded non-crop stem in the smallest DBH quartile.
        float q25 = Percentile(living.Select(t => t.Diameter), 0.25f);
        list.Add(new Treatment
        {
            Id = "T1", Description = "novice clean-up: crowded stems in the smallest DBH quartile",
            Remove = candidates.Where(t => t.Diameter <= q25 && ecology.GetCompetitionLabel(t) == "crowded")
                .Select(t => t.TreeId).ToList()
        });
        list.Add(Release("T2", $"crop-tree release: top {CropTreeReleaseNeighbours} competitors (largest Hegyi term) per crop tree", crops, candidates, CropTreeReleaseNeighbours));
        list.Add(Release("T3", $"heavy opening: top {HeavyReleaseNeighbours} competitors per crop tree", crops, candidates, HeavyReleaseNeighbours));
        list.Add(Release("T4", $"conservative: top {ConservativeNeighbours} competitor per crop tree", crops, candidates, ConservativeNeighbours));

        // T5 concentrated gap: nearest non-crop stems around one interior point,
        // until the removed stem volume reaches T2's removed volume.
        Treatment t2 = list.First(t => t.Id == "T2");
        float t2Volume = living.Where(t => t2.Remove.Contains(t.TreeId)).Sum(t => t.BiologicalStemVolumeM3);
        ForestEcologyCell centreCell = ecology.Cells
            .Where(c => Mathf.Abs(c.Center.x) < 12f && Mathf.Abs(c.Center.y) < 12f)
            .OrderByDescending(c => candidates.Where(t => ecology.GetCellIndex(t.transform.position) == ecology.GetCellIndex(new Vector3(c.Center.x, 0f, c.Center.y)))
                .Sum(t => t.BiologicalStemVolumeM3))
            .ThenBy(c => c.Center.x).ThenBy(c => c.Center.y).First();
        var gap = new Treatment { Id = "T5", Description = "concentrated gap: nearest non-crop stems to one point, volume matched to T2" };
        float taken = 0f;
        foreach (ForestTree t in candidates.OrderBy(t => Vector2.Distance(centreCell.Center,
                     new Vector2(t.transform.position.x, t.transform.position.z))).ThenBy(t => t.TreeId, StringComparer.Ordinal))
        {
            if (taken >= t2Volume) break;
            gap.Remove.Add(t.TreeId);
            taken += t.BiologicalStemVolumeM3;
        }
        list.Add(gap);
        return list;
    }

    private static Treatment Release(string id, string description, List<ForestTree> crops, List<ForestTree> candidates, int perCrop)
    {
        var chosen = new List<string>();
        foreach (ForestTree c in crops)
            foreach (ForestTree n in candidates.Where(n => Term(n, c) > 0f)
                         .OrderByDescending(n => Term(n, c)).ThenBy(n => n.TreeId, StringComparer.Ordinal).Take(perCrop))
                if (!chosen.Contains(n.TreeId)) chosen.Add(n.TreeId);
        chosen.Sort(StringComparer.Ordinal);
        return new Treatment { Id = id, Description = description, Remove = chosen };
    }

    private static float Percentile(IEnumerable<float> values, float p)
    {
        List<float> sorted = values.OrderBy(v => v).ToList();
        if (sorted.Count == 0) return 0f;
        return sorted[Mathf.Clamp(Mathf.FloorToInt(p * (sorted.Count - 1)), 0, sorted.Count - 1)];
    }

    // ------------------------------------------------------------- metrics

    private float HectareFactor => 1f / ecology.StandAreaHectares;
    private static float BasalArea(ForestTree t) => Mathf.PI * Mathf.Pow(t.Diameter / 200f, 2f);

    private float PredictedGrowth(ForestTree tree, float ci)
    {
        TreeSpeciesDefinition s = tree.Species != null ? tree.Species : ecology.ResolveSpecies();
        float potential = s.PotentialDbhGrowthCmPerYear * ecology.GetSiteProductivity(tree.transform.position)
                          * Mathf.Clamp01(1f - tree.Diameter / s.MaxDbhCm);
        return potential * (1f / (1f + ci / s.Ci50));
    }

    private string StandSummary(List<ForestTree> living)
    {
        float ba = living.Sum(BasalArea) * HectareFactor;
        float vol = living.Sum(t => t.BiologicalStemVolumeM3);
        float meanDbh = living.Average(t => t.Diameter);
        int seedTrees = living.Count(t => t.Species != null && t.Species.Maturity(t.AgeYears) > 0f);
        return $"{{\"year\": {ecology.EcologicalYear}, \"living\": {living.Count}, \"ba_m2ha\": {F(ba)}, \"standing_vol_m3\": {F(vol)}, "
            + $"\"mean_dbh_cm\": {F(meanDbh)}, \"mean_canopy\": {F(ecology.Cells.Average(c => c.Canopy))}, "
            + $"\"mean_light\": {F(ecology.Cells.Average(c => c.Light))}, \"regeneration_cells\": {ecology.RegeneratingCellCount}, "
            + $"\"seed_bearing_trees\": {seedTrees}, \"stand_area_ha\": {F(ecology.StandAreaHectares)}}}";
    }

    private IEnumerator Immediate(string context, Treatment treatment, List<string> cropIds, string baseJson)
    {
        yield return Restore(baseJson);
        List<ForestTree> living = Living();
        var removedSet = new HashSet<string>(treatment.Remove);
        List<ForestTree> removed = living.Where(t => removedSet.Contains(t.TreeId)).ToList();
        List<ForestTree> retained = living.Where(t => !removedSet.Contains(t.TreeId)).ToList();
        List<ForestTree> crops = living.Where(t => cropIds.Contains(t.TreeId) && !removedSet.Contains(t.TreeId)).ToList();
        TreeSpeciesDefinition species = ecology.ResolveSpecies();

        // Capital and money (production Work Plan quote).
        (long revenue, long cost, long minimum) harvest = Quote(removed);
        yield return Restore(baseJson);
        living = Living();
        removed = living.Where(t => removedSet.Contains(t.TreeId)).ToList();
        retained = living.Where(t => !removedSet.Contains(t.TreeId)).ToList();
        crops = living.Where(t => cropIds.Contains(t.TreeId) && !removedSet.Contains(t.TreeId)).ToList();
        long retainedRevenue = Quote(retained, false).revenue;
        yield return Restore(baseJson);
        living = Living();
        removed = living.Where(t => removedSet.Contains(t.TreeId)).ToList();
        retained = living.Where(t => !removedSet.Contains(t.TreeId)).ToList();
        crops = living.Where(t => cropIds.Contains(t.TreeId) && !removedSet.Contains(t.TreeId)).ToList();

        // Crop-tree release (production competition and growth response).
        float ciBeforeSum = 0f, ciAfterSum = 0f, gBeforeSum = 0f, gAfterSum = 0f, releasedTerms = 0f;
        foreach (ForestTree c in crops)
        {
            float before = ecology.GetCompetitionIndex(c);
            float after = retained.Where(n => n != c).Sum(n => Term(n, c));
            int competitorsRemoved = removed.Count(n => Term(n, c) > 0f);
            releasedTerms += removed.Sum(n => Term(n, c));
            float gb = PredictedGrowth(c, before), ga = PredictedGrowth(c, after);
            ciBeforeSum += before; ciAfterSum += after; gBeforeSum += gb; gAfterSum += ga;
            cropCsv.AppendLine(string.Join(",", context, treatment.Id, c.TreeId, F(c.Diameter, "0.00"), F(c.Height, "0.00"),
                F(before), F(after), competitorsRemoved, F(gb), F(ga)));
        }
        int nCrop = Mathf.Max(1, crops.Count);
        float ciBefore = ciBeforeSum / nCrop, ciAfter = ciAfterSum / nCrop;
        float gBefore = gBeforeSum / nCrop, gAfter = gAfterSum / nCrop;

        // Stand-average growth change: what the in-game forecast line reports.
        float standBefore = 0f, standAfter = 0f;
        foreach (ForestTree t in retained)
        {
            standBefore += PredictedGrowth(t, ecology.GetCompetitionIndex(t));
            standAfter += PredictedGrowth(t, retained.Where(n => n != t).Sum(n => Term(n, t)));
        }

        // Removal efficiency: does each removal touch a crop tree at all, and how much?
        int nearNoCrop = removed.Count(r => crops.All(c => Term(r, c) <= 0f));
        int lowEffect = removed.Count(r => crops.All(c =>
        {
            float ci = ecology.GetCompetitionIndex(c);
            return ci <= 0f || Term(r, c) < 0.05f * ci;   // diagnostic: under 5 % of that crop tree's competition
        }));
        float removedBa = removed.Sum(BasalArea) * HectareFactor;

        // Spatial pattern.
        var perCell = removed.GroupBy(t => ecology.GetCellIndex(t.transform.position)).ToDictionary(g => g.Key, g => g.Count());
        float clarkEvans = ClarkEvans(removed);
        float[] lightAfter = ForecastLight(retained);
        float[] lightBefore = ecology.Cells.Select(c => c.Light).ToArray();
        int bright = lightAfter.Count(l => l >= 0.20f);
        int cluster = LargestCluster(lightAfter.Select((l, i) => l - lightBefore[i] >= 0.05f).ToArray());
        float meanLight = lightAfter.Average();
        float sd = Mathf.Sqrt(lightAfter.Select(l => (l - meanLight) * (l - meanLight)).Average());
        float cv = meanLight > 0f ? sd / meanLight : 0f;

        // Stability diagnostic (production wind formula, forecast light/opening).
        float cap = ecology.MaxRecentOpeningPerCell;
        float windPeak = 0f; int highWind = 0;
        foreach (ForestTree t in retained)
        {
            int index = ecology.GetCellIndex(t.transform.position);
            if (index < 0) continue;
            perCell.TryGetValue(index, out int cut);
            float opening = Mathf.Min(cap, ecology.Cells[index].RecentOpening + cut);
            float slenderness = t.Height / Mathf.Max(0.05f, t.Diameter / 100f);
            float risk = species.StandWindSusceptibility * slenderness * (0.25f + 0.75f * lightAfter[index])
                         * (1f + species.WindOpeningWeight * opening);
            windPeak = Mathf.Max(windPeak, risk);
            if (ecology.WindRiskBandLabel(risk) == "high") highWind++;
        }
        float cropHd = crops.Count > 0 ? crops.Average(t => t.Height / Mathf.Max(0.05f, t.Diameter / 100f)) : 0f;
        int seedRetained = retained.Count(t => t.Species != null && t.Species.Maturity(t.AgeYears) > 0f);
        int seedRemoved = removed.Count(t => t.Species != null && t.Species.Maturity(t.AgeYears) > 0f);

        float retainedBa = retained.Sum(BasalArea) * HectareFactor;
        float removedVol = removed.Sum(t => t.BiologicalStemVolumeM3), retainedVol = retained.Sum(t => t.BiologicalStemVolumeM3);
        float cropCiChange = ciBefore > 0f ? (ciAfter - ciBefore) / ciBefore * 100f : 0f;
        float cropGrowthChange = gBefore > 0f ? (gAfter - gBefore) / gBefore * 100f : 0f;
        float standChange = standBefore > 0f ? (standAfter - standBefore) / standBefore * 100f : 0f;
        float releasePerBa = removedBa > 0f ? releasedTerms / removedBa : 0f;

        immediateCsv.AppendLine(string.Join(",", context, treatment.Id, removed.Count, F(removedBa), F(removedVol), retained.Count, F(retainedBa),
            F(retainedVol), F(harvest.revenue / 100.0, "0.00"), F(harvest.cost / 100.0, "0.00"), F(harvest.minimum / 100.0, "0.00"),
            F((harvest.revenue - harvest.cost) / 100.0, "0.00"), F(retainedRevenue / 100.0, "0.00"),
            crops.Count, F(ciBefore), F(ciAfter), F(cropCiChange, "0.00"), F(gBefore), F(gAfter), F(cropGrowthChange, "0.00"),
            F(standChange, "0.00"), nearNoCrop, lowEffect, F(releasePerBa),
            perCell.Count, perCell.Count > 0 ? perCell.Values.Max() : 0, F(clarkEvans), F(meanLight), F(lightAfter.Max()), bright,
            cluster, F(cv), F(windPeak, "0.00"), highWind, F(cropHd, "0.0"), seedRetained, seedRemoved));
        Debug.Log($"RESIDUAL_STAND_IMMEDIATE {context} {treatment.Id} removed={removed.Count} vol={F(removedVol, "0.00")} "
            + $"cropCI {F(ciBefore, "0.00")}->{F(ciAfter, "0.00")} cropGrowth {F(cropGrowthChange, "0.0")}% standGrowth {F(standChange, "0.0")}% "
            + $"cells={perCell.Count} R={F(clarkEvans, "0.00")} cluster={cluster} net={F((harvest.revenue - harvest.cost) / 100.0, "0.00")}");

        lastRow = $"{{\"context\": \"{context}\", \"treatment\": \"{treatment.Id}\", \"removed_n\": {removed.Count}, \"removed_ba_m2ha\": {F(removedBa)}, "
            + $"\"removed_vol_m3\": {F(removedVol)}, \"retained_n\": {retained.Count}, \"retained_ba_m2ha\": {F(retainedBa)}, \"retained_vol_m3\": {F(retainedVol)}, "
            + $"\"economics\": {{\"gross_revenue_eur\": {F(harvest.revenue / 100.0, "0.00")}, \"external_cost_eur\": {F(harvest.cost / 100.0, "0.00")}, "
            + $"\"minimum_adjustment_eur\": {F(harvest.minimum / 100.0, "0.00")}, \"net_eur\": {F((harvest.revenue - harvest.cost) / 100.0, "0.00")}, "
            + $"\"retained_gross_roadside_value_eur\": {F(retainedRevenue / 100.0, "0.00")}}}, "
            + $"\"crop_release\": {{\"crop_n\": {crops.Count}, \"ci_before\": {F(ciBefore)}, \"ci_after\": {F(ciAfter)}, \"ci_change_pct\": {F(cropCiChange, "0.00")}, "
            + $"\"growth_before_cm\": {F(gBefore)}, \"growth_after_cm\": {F(gAfter)}, \"growth_change_pct\": {F(cropGrowthChange, "0.00")}, "
            + $"\"removals_near_no_crop\": {nearNoCrop}, \"removals_low_effect\": {lowEffect}, \"release_per_m2ba\": {F(releasePerBa)}}}, "
            + $"\"stand_growth_change_pct\": {F(standChange, "0.00")}, "
            + $"\"spatial\": {{\"cells_touched\": {perCell.Count}, \"max_removed_per_cell\": {(perCell.Count > 0 ? perCell.Values.Max() : 0)}, "
            + $"\"clark_evans_R\": {F(clarkEvans)}, \"mean_light_after\": {F(meanLight)}, \"max_light_after\": {F(lightAfter.Max())}, "
            + $"\"cells_light_ge_0_20\": {bright}, \"largest_opened_cluster_cells\": {cluster}, \"light_cv_after\": {F(cv)}}}, "
            + $"\"stability\": {{\"wind_peak_after\": {F(windPeak, "0.00")}, \"retained_high_wind\": {highWind}, \"crop_mean_hd\": {F(cropHd, "0.0")}}}, "
            + $"\"seed\": {{\"seed_trees_retained\": {seedRetained}, \"seed_trees_removed\": {seedRemoved}}}}}";
    }

    // Marteloscope-style tree list and per-cell table (read-only).
    private void Inventory(string context, List<ForestTree> living, List<string> cropIds)
    {
        ecology.InvalidateCompetition();
        foreach (ForestTree t in living)
        {
            float risk = ecology.GetWindRisk(t);
            treeCsv.AppendLine(string.Join(",", context, t.TreeId, t.Species != null ? t.Species.SpeciesId : "", F(t.transform.position.x, "0.00"),
                F(t.transform.position.z, "0.00"), ecology.GetCellIndex(t.transform.position), t.AgeYears, F(t.Diameter, "0.00"), F(t.Height, "0.00"),
                F(t.CrownRadius, "0.00"), F(t.Height / Mathf.Max(0.05f, t.Diameter / 100f), "0.0"), F(t.BiologicalStemVolumeM3),
                F(ecology.GetCompetitionIndex(t)), ecology.GetCompetitionLabel(t).Replace(' ', '_').Replace('/', '-'),
                F(ecology.GetCurrentSuppression(t)), F(risk, "0.00"), ecology.WindRiskBandLabel(risk),
                t.Species != null && t.Species.Maturity(t.AgeYears) > 0f ? 1 : 0, cropIds.Contains(t.TreeId) ? 1 : 0));
        }
        for (int i = 0; i < ecology.Cells.Length; i++)
        {
            ForestEcologyCell c = ecology.Cells[i];
            List<ForestTree> inCell = living.Where(t => ecology.GetCellIndex(t.transform.position) == i).ToList();
            var bands = c.Regeneration.Where(r => r != null && r.Species != null && r.Density > 0f).ToList();
            string label = ((char)('A' + i % ecology.CellsPerAxis)).ToString() + (i / ecology.CellsPerAxis + 1);
            cellCsv.AppendLine(string.Join(",", context, i, label, F(c.Center.x, "0.0"), F(c.Center.y, "0.0"), F(c.Light), F(c.Canopy),
                F(c.RecentOpening), inCell.Count, F(inCell.Sum(BasalArea) / (ecology.CellSizeMeters * ecology.CellSizeMeters / 10000f)),
                string.Join("|", bands.Select(b => b.Species.SpeciesId).Distinct()), F(bands.Sum(b => b.Density)),
                F(bands.Count > 0 ? bands.Max(b => b.Height) : 0f, "0.00")));
        }
    }

    private void NeighbourTable(string context, List<ForestTree> living, List<string> cropIds, List<Treatment> treatments)
    {
        ecology.InvalidateCompetition();
        foreach (ForestTree c in living.Where(t => cropIds.Contains(t.TreeId)))
        {
            float ci = ecology.GetCompetitionIndex(c);
            int rank = 0;
            foreach (ForestTree n in living.Where(n => n != c && Term(n, c) > 0f)
                         .OrderByDescending(n => Term(n, c)).ThenBy(n => n.TreeId, StringComparer.Ordinal))
            {
                rank++;
                float term = Term(n, c);
                string removedIn = string.Join("|", treatments.Where(t => t.Remove.Contains(n.TreeId)).Select(t => t.Id));
                neighbourCsv.AppendLine(string.Join(",", context, c.TreeId, F(c.Diameter, "0.00"), F(c.Height, "0.00"), F(ci),
                    n.TreeId, F(n.Diameter, "0.00"), F(n.Height, "0.00"), F(Distance(n, c), "0.00"), F(term),
                    F(ci > 0f ? term / ci : 0f), rank, ecology.GetCompetitionLabel(n).Replace(' ', '_').Replace('/', '-'),
                    cropIds.Contains(n.TreeId) ? 1 : 0, removedIn));
            }
        }
    }

    // Production Work Plan quote for felling `trees` (caller restores the world).
    private (long revenue, long cost, long minimum) Quote(List<ForestTree> trees, bool required = true)
    {
        if (trees.Count == 0) return (0, 0, 0);
        manager.PlanningFellingOutcome = FellingMaterialOutcome.SellAndExtract;
        foreach (ForestTree t in trees) marking.Mark(t, TreeMarkType.Fell, false);
        int added = manager.AddMarkedTreesToWorkPlan();
        Check(added == trees.Count, $"work plan accepted {added} of {trees.Count} marked trees");
        ScenarioHarvestJob job = manager.GetHarvestQuote(false);
        if (!job.Eligible)
        {
            Check(!required, "harvest quote ineligible: " + job.Problem);
            Debug.Log("RESIDUAL_STAND_QUOTE_INELIGIBLE " + job.Problem);
            return (-100, -100, -100);
        }
        long minimum = job.Resolution != null ? job.Resolution.Quote.Costs.MinimumJobAdjustmentCents : 0;
        return (job.RevenueCents, job.CostCents, minimum);
    }

    private void ApplyThroughWorkPlan(Treatment t, List<string> cropIds)
    {
        List<ForestTree> living = Living();
        foreach (ForestTree c in living.Where(x => cropIds.Contains(x.TreeId))) marking.Mark(c, TreeMarkType.CropTree, false);
        if (t.Remove.Count == 0) return;
        manager.PlanningFellingOutcome = FellingMaterialOutcome.SellAndExtract;
        foreach (ForestTree x in living.Where(x => t.Remove.Contains(x.TreeId))) marking.Mark(x, TreeMarkType.Fell, false);
        Check(manager.AddMarkedTreesToWorkPlan() == t.Remove.Count, "work plan did not take every felling for " + t.Id);
        Check(manager.ApprovePendingWork(), $"approval failed for {t.Id}: {manager.Feedback}");
    }

    private IEnumerator RunToYearHash(Treatment t, List<string> cropIds, int year)
    {
        yield return Restore(originalJson);
        ApplyThroughWorkPlan(t, cropIds);
        while (ecology.EcologicalYear < year)
        {
            Check(manager.AdvanceYear(), "determinism advance failed");
            yield return null;
        }
        yield return null; yield return null;
        lastHash = ScenarioReferenceArchive.WorldHash(saves.CaptureData());
    }

    private string LongTermRow(string id, List<string> cropIds)
    {
        ecology.InvalidateCompetition();
        List<ForestTree> living = Living();
        List<ForestTree> crops = living.Where(t => cropIds.Contains(t.TreeId)).ToList();
        float ba = living.Sum(BasalArea) * HectareFactor;
        float vol = living.Sum(t => t.BiologicalStemVolumeM3);
        float cropDbh = crops.Count > 0 ? crops.Average(t => t.Diameter) : 0f;
        float cropInc = crops.Count > 0 ? crops.Average(t => ecology.GetAnnualDbhGrowth(t)) : 0f;
        float cropCi = crops.Count > 0 ? crops.Average(t => ecology.GetCompetitionIndex(t)) : 0f;
        float standInc = living.Count > 0 ? living.Average(t => ecology.GetAnnualDbhGrowth(t)) : 0f;
        float canopy = ecology.Cells.Average(c => c.Canopy), light = ecology.Cells.Average(c => c.Light);
        longCsv.AppendLine(string.Join(",", id, ecology.EcologicalYear, living.Count, F(ba), F(vol), crops.Count, F(cropDbh), F(cropInc),
            F(cropCi), F(standInc), F(canopy), F(light), ecology.RegeneratingCellCount, F(ecology.MaxWindRisk, "0.00"),
            F(manager.CashCents / 100.0, "0.00")));
        Debug.Log($"RESIDUAL_STAND_LONG {id} Y{ecology.EcologicalYear} living={living.Count} ba={F(ba, "0.00")} cropDbh={F(cropDbh, "0.00")} "
            + $"cropInc={F(cropInc, "0.000")} standInc={F(standInc, "0.000")} light={F(light, "0.000")} regen={ecology.RegeneratingCellCount}");
        return $"{{\"treatment\": \"{id}\", \"year\": {ecology.EcologicalYear}, \"living\": {living.Count}, \"ba_m2ha\": {F(ba)}, "
            + $"\"standing_vol_m3\": {F(vol)}, \"crop_alive\": {crops.Count}, \"crop_mean_dbh_cm\": {F(cropDbh)}, "
            + $"\"crop_mean_increment_cm\": {F(cropInc)}, \"crop_mean_ci\": {F(cropCi)}, \"stand_mean_increment_cm\": {F(standInc)}, "
            + $"\"mean_canopy\": {F(canopy)}, \"mean_light\": {F(light)}, \"regeneration_cells\": {ecology.RegeneratingCellCount}, "
            + $"\"max_wind\": {F(ecology.MaxWindRisk, "0.00")}, \"cash_eur\": {F(manager.CashCents / 100.0, "0.00")}}}";
    }

    // Same fractional-cover model as ForestEcologyController.RecomputeCanopy.
    private float[] ForecastLight(List<ForestTree> retained)
    {
        var light = new float[ecology.Cells.Length];
        for (int i = 0; i < ecology.Cells.Length; i++)
        {
            Vector2 centre = ecology.Cells[i].Center;
            float gap = 1f;
            foreach (ForestTree t in retained)
            {
                float d = Vector2.Distance(centre, new Vector2(t.transform.position.x, t.transform.position.z));
                float reach = t.CrownRadius * ForestEcologyController.CanopyShadeReachPerCrownRadius + ecology.CellSizeMeters * 0.5f;
                float lateral = Mathf.Clamp01(1f - d / reach);
                float vertical = Mathf.Clamp01(t.Height / 8f);
                gap *= 1f - Mathf.Clamp01(lateral * vertical);
            }
            light[i] = gap;
        }
        return light;
    }

    // Clark-Evans aggregation index of removed stems (no edge correction):
    // R < 1 clustered, R ≈ 1 random, R > 1 regular. Diagnostic only.
    private float ClarkEvans(List<ForestTree> removed)
    {
        if (removed.Count < 2) return 0f;
        float area = ecology.StandBounds.width * ecology.StandBounds.height;
        float meanNearest = removed.Average(a => removed.Where(b => b != a).Min(b => Distance(a, b)));
        float expected = 0.5f / Mathf.Sqrt(removed.Count / area);
        return meanNearest / expected;
    }

    private int LargestCluster(bool[] opened)
    {
        int n = ecology.CellsPerAxis, best = 0;
        var seen = new bool[opened.Length];
        for (int i = 0; i < opened.Length; i++)
        {
            if (!opened[i] || seen[i]) continue;
            int size = 0;
            var stack = new Stack<int>(); stack.Push(i); seen[i] = true;
            while (stack.Count > 0)
            {
                int c = stack.Pop(); size++;
                int x = c % n, z = c / n;
                foreach ((int dx, int dz) in new[] { (1, 0), (-1, 0), (0, 1), (0, -1) })
                {
                    int nx = x + dx, nz = z + dz;
                    if (nx < 0 || nz < 0 || nx >= n || nz >= n) continue;
                    int k = nz * n + nx;
                    if (opened[k] && !seen[k]) { seen[k] = true; stack.Push(k); }
                }
            }
            best = Mathf.Max(best, size);
        }
        return best;
    }

    private void WriteOutputs()
    {
        File.WriteAllText(Path.Combine(output, "residual-stand.json"), json.ToString());
        File.WriteAllText(Path.Combine(output, "residual-stand-immediate.csv"), immediateCsv.ToString());
        File.WriteAllText(Path.Combine(output, "residual-stand-longterm.csv"), longCsv.ToString());
        File.WriteAllText(Path.Combine(output, "residual-stand-croptrees.csv"), cropCsv.ToString());
        File.WriteAllText(Path.Combine(output, "residual-stand-neighbours.csv"), neighbourCsv.ToString());
        File.WriteAllText(Path.Combine(output, "residual-stand-trees.csv"), treeCsv.ToString());
        File.WriteAllText(Path.Combine(output, "residual-stand-cells.csv"), cellCsv.ToString());
        using (SHA256 sha = SHA256.Create())
        {
            string all = json.ToString() + immediateCsv + longCsv + cropCsv + neighbourCsv + treeCsv + cellCsv;
            string digest = BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(all))).Replace("-", "");
            Debug.Log("RESIDUAL_STAND_OUTPUT_SHA256 " + digest);
        }
    }
}
#endif
